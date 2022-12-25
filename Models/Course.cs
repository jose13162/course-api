using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Models {
	public class Course {
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public virtual ICollection<Lesson> Lessons { get; set; }
		public virtual ICollection<CourseCategory> CourseCategories { get; set; }
		public virtual Cover Cover { get; set; }
	}
}