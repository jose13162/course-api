using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using course_api.Dto;
using course_api.Interface;
using course_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace course_api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : Controller {
		private readonly IMapper _mapper;
		private readonly ICourseRepository _courseRepository;
		private readonly ICategoryRepository _categoryRepository;

		public CourseController(IMapper mapper, ICourseRepository courseRepository, ICategoryRepository categoryRepository) {
			this._mapper = mapper;
			this._courseRepository = courseRepository;
			this._categoryRepository = categoryRepository;
		}

		[HttpGet]
		public IActionResult GetCourses() {
			var courses = this._courseRepository.GetCourses();
			var mappedCourses = this._mapper.Map<ICollection<CourseDto>>(courses);

			return Ok(mappedCourses);
		}

		[HttpGet("{courseId}")]
		public IActionResult GetCourse(Guid courseId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				return NotFound();
			}

			var course = this._courseRepository.GetCourse(courseId);
			var mappedCourse = this._mapper.Map<CourseDto>(course);

			return Ok(mappedCourse);
		}

		[HttpPost]
		public IActionResult CreateCourse([FromBody] CourseDto course) {
			var mappedCourse = this._mapper.Map<Course>(course);

			if (!this._courseRepository.CreateCourse(mappedCourse)) {
				ModelState.AddModelError("", "Something went wrong saving the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPut]
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
		public IActionResult DeleteCourse(Guid courseId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				return NotFound();
			}

			var course = this._courseRepository.GetCourse(courseId); ;

			if (!this._courseRepository.DeleteCourse(course)) {
				ModelState.AddModelError("", "Something went wrong deleting the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPost("categories")]
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
	}
}