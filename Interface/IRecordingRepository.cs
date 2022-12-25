using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Interface {
	public interface IRecordingRepository {
		Recording GetRecording(Guid recordingId);
		bool RecordingExists(Guid recordingId);
		bool CreateRecording(Recording recording);
		bool DeleteRecording(Recording recording);
		bool Save();
	}
}