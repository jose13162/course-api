using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Models {
	public class Cover {
		public Guid Id { get; set; }
		public Guid CourseId { get; set; }
		public string Filename { get; set; }
		public string Url { get; set; }
		public virtual Course Course { get; set; }
	}
}