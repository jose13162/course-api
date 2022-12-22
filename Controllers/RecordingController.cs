using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Interface;
using course_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace course_api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class RecordingController : Controller {
		private readonly IFileUploader _fileUploader;
		private readonly IRecordingRepository _recordingRepository;
		private readonly ILessonRepository _lessonRepository;

		public RecordingController(IFileUploader fileUploader, IRecordingRepository recordingRepository, ILessonRepository lessonRepository) {
			this._fileUploader = fileUploader;
			this._recordingRepository = recordingRepository;
			this._lessonRepository = lessonRepository;
		}

		[HttpPost]
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
				ModelState.AddModelError("", "Something went wrong uploading the recording");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpDelete("{recordingId}")]
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