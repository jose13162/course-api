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
	public class LessonController : Controller {
		private readonly IMapper _mapper;
		private readonly IFileUploader _fileUploader;
		private readonly ILessonRepository _lessonRepository;
		private readonly ICourseRepository _courseRepository;
		private readonly IRecordingRepository _recordingRepository;

		public LessonController(IMapper mapper, IFileUploader fileUploader, ILessonRepository lessonRepository, ICourseRepository courseRepository, IRecordingRepository recordingRepository) {
			this._mapper = mapper;
			this._fileUploader = fileUploader;
			this._lessonRepository = lessonRepository;
			this._courseRepository = courseRepository;
			this._recordingRepository = recordingRepository;
		}

		[HttpGet]
		public IActionResult GetLessons([FromQuery] Guid courseId) {
			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			var lessons = this._lessonRepository.GetLessons(courseId);
			var mappedLessons = this._mapper.Map<ICollection<LessonDto>>(lessons);

			return Ok(mappedLessons);
		}

		[HttpGet("{lessonId}")]
		public IActionResult GetLesson(Guid lessonId) {
			if (!this._lessonRepository.LessonExists(lessonId)) {
				ModelState.AddModelError("", "The lesson does not exist");

				return NotFound(ModelState);
			}

			var lesson = this._lessonRepository.GetLesson(lessonId);
			var mappedLesson = this._mapper.Map<LessonDto>(lesson);

			return Ok(mappedLesson);
		}

		[HttpPost]
		public IActionResult CreateLesson([FromQuery] Guid courseId, [FromBody] LessonDto lesson) {
			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			var mappedLesson = this._mapper.Map<Lesson>(lesson);
			mappedLesson.Course = this._courseRepository.GetCourse(courseId);

			if (!this._lessonRepository.CreateLesson(mappedLesson)) {
				ModelState.AddModelError("", "Something went wrong saving the lesson");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPut]
		public IActionResult UpdateLesson([FromQuery] Guid lessonId, [FromQuery] Guid courseId, [FromBody] LessonDto lesson) {
			if (lessonId != lesson.Id) {
				ModelState.AddModelError("", "The lessonId from query does not match the body id");

				return BadRequest(ModelState);
			}

			if (!this._lessonRepository.LessonExists(lessonId)) {
				ModelState.AddModelError("", "The lesson does not exist");

				return NotFound(ModelState);
			}

			if (!this._courseRepository.CourseExists(courseId)) {
				ModelState.AddModelError("", "The course does not exist");

				return NotFound(ModelState);
			}

			var mappedLesson = this._mapper.Map<Lesson>(lesson);
			mappedLesson.Course = this._courseRepository.GetCourse(courseId);

			if (!this._lessonRepository.UpdateLesson(mappedLesson)) {
				ModelState.AddModelError("", "Something went wrong updating the lesson");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("{lessonId}")]
		public IActionResult DeleteLesson(Guid lessonId) {
			if (!this._lessonRepository.LessonExists(lessonId)) {
				ModelState.AddModelError("", "The lesson does not exist");

				return NotFound(ModelState);
			}

			var lesson = this._lessonRepository.GetLesson(lessonId);

			if (!this._lessonRepository.DeleteLesson(lesson)) {
				ModelState.AddModelError("", "Something went wrong deleting the lesson");

				return BadRequest(ModelState);
			}

			if (lesson.Recording != null) {
				this._fileUploader.Delete(lesson.Recording.FileName);
			}

			return Ok();
		}

		[HttpPost("recordings")]
		public IActionResult CreateRecording([FromQuery] Guid lessonId, IFormFile recordingFile) {
			if (!this._lessonRepository.LessonExists(lessonId)) {
				ModelState.AddModelError("", "The lesson does not exist");

				return NotFound(ModelState);
			}

			var lesson = this._lessonRepository.GetLesson(lessonId);

			if (lesson.Recording != null) {
				this._fileUploader.Delete(lesson.Recording.FileName);
			}

			var (fileName, url) = this._fileUploader.Upload(recordingFile);
			var recording = new Recording() {
				FileName = fileName,
				Url = url,
			};
			recording.Lesson = this._lessonRepository.GetLesson(lessonId);

			if (!this._recordingRepository.CreateRecording(recording)) {
				ModelState.AddModelError("", "Something went wrong saving the recording");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("recordings/{recordingId}")]
		public IActionResult DeleteRecording(Guid recordingId) {
			if (!this._recordingRepository.RecordingExists(recordingId)) {
				ModelState.AddModelError("", "The recording does not exist");

				return NotFound(ModelState);
			}

			var recording = this._recordingRepository.GetRecording(recordingId);

			if (!this._recordingRepository.DeleteRecording(recording)) {
				ModelState.AddModelError("", "Something went wrong deleting the recording");

				return BadRequest(ModelState);
			}

			this._fileUploader.Delete(recording.FileName);

			return Ok();
		}
	}
}