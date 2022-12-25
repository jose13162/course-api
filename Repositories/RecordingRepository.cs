using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;

namespace course_api.Repositories {
	public class RecordingRepository : IRecordingRepository {
		private readonly DataContext _context;

		public RecordingRepository(DataContext context) {
			this._context = context;
		}

		public Recording GetRecording(Guid recordingId) {
			return this._context.Recordings.Find(recordingId);
		}

		public bool RecordingExists(Guid recordingId) {
			return this._context.Recordings.Any((recording) => recording.Id == recordingId);
		}

		public bool CreateRecording(Recording recording) {
			this._context.Recordings.Add(recording);

			return this.Save();
		}

		public bool DeleteRecording(Recording recording) {
			this._context.Recordings.Remove(recording);

			return this.Save();
		}

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}
	}
}