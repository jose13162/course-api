using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using course_api.Dto;
using course_api.Helper;
using course_api.Interface;
using course_api.Models;
using course_api.Validators.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace course_api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : Controller {
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly IEmailSender _emailSender;
		private readonly IFileUploader _fileUploader;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IAvatarRepository _avatarRepository;

		public UserController(IMapper mapper, IConfiguration configuration, IEmailSender emailSender, IFileUploader fileUploader, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAvatarRepository avatarRepository) {
			this._mapper = mapper;
			this._configuration = configuration;
			this._emailSender = emailSender;
			this._fileUploader = fileUploader;
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._avatarRepository = avatarRepository;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterValidator body) {
			var user = new ApplicationUser();
			user.Email = body.Email;
			user.FirstName = body.FirstName;
			user.LastName = body.LastName;
			user.UserName = body.Username;

			var result = await this._userManager.CreateAsync(user, body.Password);

			if (!result.Succeeded) {
				return BadRequest(result.Errors);
			}

			var roleExists = await this._roleManager.RoleExistsAsync(body.Role);

			if (!roleExists) {
				var identityRole = new IdentityRole() {
					Name = body.Role,
					NormalizedName = body.Role.Normalize()
				};
				var createRoleResult = await this._roleManager.CreateAsync(identityRole);

				if (!createRoleResult.Succeeded) {
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}

			var addToRoleResult = await this._userManager.AddToRoleAsync(user, body.Role);

			if (!addToRoleResult.Succeeded) {
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

			var controllerName = nameof(UserController).Remove(nameof(UserController).IndexOf("Controller"));
			var actionName = nameof(UserController.ConfirmEmail);
			var url = Url.Action(actionName, controllerName, new {
				userId = user.Id,
				token = token
			}, protocol: HttpContext.Request.Scheme);

			var message = new MailMessage() {
				To = { new MailAddress(user.Email) },
				Subject = "Email confirmation",
				Body = $"Click here to confirm your email: <a href=\"{url}\">{url}</a>",
				IsBodyHtml = true
			};

			this._emailSender.Send(message);

			return Ok();
		}

		[HttpGet("confirm-email")]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string token) {
			var user = await this._userManager.FindByIdAsync(userId);
			var result = await this._userManager.ConfirmEmailAsync(user, token);

			if (!result.Succeeded) {
				ModelState.AddModelError("", "Something went wrong confirming the email");

				return BadRequest(ModelState);
			}

			return Json("Email confirmed");
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginValidator body) {
			var user = await this._userManager.FindByEmailAsync(body.Email);
			var isEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);

			if (!isEmailConfirmed) {
				ModelState.AddModelError("", "The user's email must be confirmed");

				return StatusCode(403, ModelState);
			}

			var isPasswordCorrect = await this._userManager.CheckPasswordAsync(user, body.Password);

			if (user == null | !isPasswordCorrect) {
				ModelState.AddModelError("", "Invalid login credentials");

				return StatusCode(403, ModelState);
			}

			var roles = await this._userManager.GetRolesAsync(user);
			var role = roles.FirstOrDefault();
			var claims = new ClaimsIdentity();
			claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
			claims.AddClaim(new Claim(ClaimTypes.Role, role));
			claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));

			var key = Encoding.UTF8.GetBytes(this._configuration.GetValue<string>("Jwt:Secret"));
			var issuer = this._configuration.GetValue<string>("Jwt:Issuer");
			var audience = this._configuration.GetValue<string>("Jwt:audience");
			var tokenDescriptor = new SecurityTokenDescriptor() {
				Subject = claims,
				Expires = DateTime.UtcNow.AddDays(30),
				Issuer = issuer,
				Audience = audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var jwtToken = tokenHandler.WriteToken(token);
			var stringToken = tokenHandler.WriteToken(token);

			return Ok(new {
				token = stringToken
			});
		}

		[HttpPut]
		[Authorize]
		public async Task<IActionResult> UpdateAuthenticatedUser([FromBody] UpdateProfileValidator body) {
			var user = await this._userManager.GetUserAsync(User);

			if (!string.IsNullOrEmpty(body.Username)) {
				user.UserName = body.Username;
			}

			if (!string.IsNullOrEmpty(body.FirstName)) {
				user.FirstName = body.FirstName;
			}

			if (!string.IsNullOrEmpty(body.LastName)) {
				user.LastName = body.LastName;
			}

			var result = await this._userManager.UpdateAsync(user);

			if (!result.Succeeded) {
				return StatusCode(500, result);
			}

			var mappedUser = this._mapper.Map<ApplicationUserDto>(user);

			return Ok(mappedUser);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAuthenticatedUser() {
			var user = await this._userManager.GetUserAsync(User);
			this._avatarRepository.LoadAvatarFromUser(user);
			var mappedUser = this._mapper.Map<ApplicationUserDto>(user);

			return Ok(mappedUser);
		}

		[HttpPost("avatar")]
		[Authorize]
		public async Task<IActionResult> SetAvatar(IFormFile avatarFile) {
			var user = await this._userManager.GetUserAsync(User);
			var (fileName, url) = this._fileUploader.Upload(avatarFile);
			var avatar = new Avatar() {
				FileName = fileName,
				Url = url,
				User = user
			};

			var existingAvatar = this._avatarRepository.GetAvatarFromUser(user);

			if (existingAvatar != null) {
				this._fileUploader.Delete(existingAvatar.FileName);
			}

			if (!this._avatarRepository.CreateAvatar(avatar)) {
				ModelState.AddModelError("", "Something went wrong creating the avatar");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("avatar")]
		[Authorize]
		public async Task<IActionResult> DeleteAvatar() {
			var user = await this._userManager.GetUserAsync(User);
			var avatar = this._avatarRepository.GetAvatarFromUser(user)!;

			if (!this._avatarRepository.DeleteAvatar(avatar)) {
				ModelState.AddModelError("", "Something went wrong deleting the avatar");

				return BadRequest(ModelState);
			}

			this._fileUploader.Delete(avatar.FileName);

			return Ok();
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordValidator body) {
			var user = await this._userManager.FindByEmailAsync(body.Email);

			if (user == null) {
				return BadRequest();
			}

			var isEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);

			if (!isEmailConfirmed) {
				ModelState.AddModelError("", "The user's email must be confirmed");

				return StatusCode(403, ModelState);
			}

			var token = await this._userManager.GeneratePasswordResetTokenAsync(user);

			var url = $"{body.RedirectUrl.TrimEnd('/')}/?email={user.Email}&token={HttpUtility.UrlEncode(token)}";
			var message = new MailMessage() {
				To = { user.Email },
				Subject = "Password Reset",
				Body = $"<p>Ignore this email if you did not try to reset your password</p><br /><a href=\"{url}\">Reset Password<a/>",
				IsBodyHtml = true
			};

			this._emailSender.Send(message);

			return Ok(token);
		}

		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordValidator body) {
			var user = await this._userManager.FindByEmailAsync(body.Email);

			var changePasswordResult = await this._userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(body.Token), body.NewPassword);

			if (!changePasswordResult.Succeeded) {
				return BadRequest(changePasswordResult.Errors);
			}

			return Ok();
		}
	}
}