using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using course_api.Interface;
using course_api.Models;
using course_api.Validators.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace course_api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : Controller {
		private readonly IConfiguration _configuration;
		private readonly IEmailSender _emailSender;
		private UserManager<ApplicationUser> _userManager;

		public UserController(IConfiguration configuration, IEmailSender emailSender, UserManager<ApplicationUser> userManager) {
			this._configuration = configuration;
			this._emailSender = emailSender;
			this._userManager = userManager;
		}

		[HttpPost("register")]
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

			var claims = new ClaimsIdentity();
			claims.AddClaim(new Claim("Id", user.Id));
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
	}
}