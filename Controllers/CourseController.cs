using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using course_api.Dto;
using course_api.Interface;
using course_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace course_api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : Controller {
		private readonly IMapper _mapper;
		private readonly IFileUploader _fileUploader;
		private readonly ICourseRepository _courseRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICoverRepository _coverRepository;

		public CourseController(IMapper mapper, IFileUploader fileUploader, ICourseRepository courseRepository, ICategoryRepository categoryRepository, ICoverRepository coverRepository) {
			this._mapper = mapper;
			this._fileUploader = fileUploader;
			this._courseRepository = courseRepository;
			this._categoryRepository = categoryRepository;
			this._coverRepository = coverRepository;
		}

		[HttpGet]
		[Authorize]
		public IActionResult GetCourses() {
			var courses = this._courseRepository.GetCourses();
			var mappedCourses = this._mapper.Map<ICollection<CourseDto>>(courses);

			return Ok(mappedCourses);
		}

		[HttpGet("{courseId}")]
		[Authorize]
		public IActionResult GetCourse(Guid courseId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				return NotFound();
			}

			var course = this._courseRepository.GetCourse(courseId);
			var mappedCourse = this._mapper.Map<CourseDto>(course);

			return Ok(mappedCourse);
		}

		[HttpPost]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult CreateCourse([FromBody] CourseDto course) {
			var mappedCourse = this._mapper.Map<Course>(course);

			if (!this._courseRepository.CreateCourse(mappedCourse)) {
				ModelState.AddModelError("", "Something went wrong saving the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPut]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult UpdateCourse([FromQuery] Guid courseId, [FromBody] CourseDto course) {
			if (courseId != course.Id) {
				ModelState.AddModelError("", "The lessonId from query does not match the body id");

				return BadRequest(ModelState);
			}

			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			var mappedCourse = this._mapper.Map<Course>(course);

			if (!this._courseRepository.UpdateCourse(mappedCourse)) {
				ModelState.AddModelError("", "Something went wrong updating the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("{courseId}")]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult DeleteCourse(Guid courseId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				return NotFound();
			}

			var course = this._courseRepository.GetCourse(courseId); ;

			if (!this._courseRepository.DeleteCourse(course)) {
				ModelState.AddModelError("", "Something went wrong deleting the course");

				return BadRequest(ModelState);
			}

			if (course.Cover != null) {
				this._fileUploader.Delete(course.Cover.FileName);
			}

			return Ok();
		}

		[HttpPost("categories")]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult AddCategory(Guid courseId, Guid categoryId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			if (!this._categoryRepository.CategoryExists(categoryId)) {
				ModelState.AddModelError("", "The category does not exist");

				return NotFound(ModelState);
			}

			var course = this._courseRepository.GetCourse(courseId);
			var category = this._categoryRepository.GetCategory(categoryId);
			var courseCategory = new CourseCategory() {
				Course = course,
				Category = category
			};

			if (!this._courseRepository.AddCategory(courseCategory)) {
				ModelState.AddModelError("", "Something went wrong adding the category to the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("categories")]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult RemoveCategory([FromQuery] Guid courseId, [FromQuery] Guid categoryId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			if (!this._categoryRepository.CategoryExists(categoryId)) {
				ModelState.AddModelError("", "The category does not exist");

				return NotFound(ModelState);
			}

			var course = this._courseRepository.GetCourse(courseId);
			var courseCategory = this._courseRepository.GetCourseCategory(course, categoryId);

			if (!this._courseRepository.RemoveCategory(courseCategory)) {
				ModelState.AddModelError("", "Something went wrong adding the category to the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPost("cover")]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult CreateCover([FromQuery] Guid courseId, IFormFile coverFile) {
			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			var course = this._courseRepository.GetCourse(courseId);

			if (course.Cover != null) {
				this._fileUploader.Delete(course.Cover.FileName);
			}

			var (fileName, url) = this._fileUploader.Upload(coverFile);
			var cover = new Cover() {
				FileName = fileName,
				Url = url
			};
			cover.Course = course;

			if (!this._coverRepository.CreateCover(cover)) {
				ModelState.AddModelError("", "Something went wrong saving the cover");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("cover/{coverId}")]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult DeleteCover(Guid coverId) {
			if (!this._coverRepository.CoverExists(coverId)) {
				ModelState.AddModelError("", "The cover does not exist");

				return NotFound(ModelState);
			}

			var cover = this._coverRepository.GetCover(coverId);

			if (!this._coverRepository.DeleteCover(cover)) {
				ModelState.AddModelError("", "Something went wrong deleting the cover");

				return BadRequest(ModelState);
			}

			this._fileUploader.Delete(cover.FileName);

			return Ok();
		}
	}
}