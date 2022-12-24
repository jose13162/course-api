using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Models {
	public class CourseCategory {
		public Guid CourseId { get; set; }
		public Guid CategoryId { get; set; }
		public virtual Course Course { get; set; }
		public virtual Category Category { get; set; }
	}
}