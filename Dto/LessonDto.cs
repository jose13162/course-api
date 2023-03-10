using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Dto {
	public class LessonDto {
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public RecordingDto? Recording { get; set; }
	}
}