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

		public CourseController(IMapper mapper, ICourseRepository courseRepository) {
			this._mapper = mapper;
			this._courseRepository = courseRepository;
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
		public IActionResult CreateCourse([FromBody] CourseDto mappedCourse) {
			var course = this._mapper.Map<Course>(mappedCourse);

			if (!this._courseRepository.CreateCourse(course)) {
				ModelState.AddModelError("", "Something went wrong saving the course");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPut]
		public IActionResult UpdateCourse([FromQuery] Guid courseId, [FromBody] CourseDto mappedCourse) {
			if (!this._courseRepository.CourseExists(courseId)) {
				return NotFound();
			}

			var course = this._mapper.Map<Course>(mappedCourse);

			if (!this._courseRepository.UpdateCourse(course)) {
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
	}
}