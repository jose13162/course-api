using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Models {
	public class Recording {
		public Guid Id { get; set; }
		public Guid LessonId { get; set; }
		public string FileName { get; set; }
		public string Url { get; set; }
		public virtual Lesson Lesson { get; set; }
	}
}