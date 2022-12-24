using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Models {
	public class Category {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public virtual ICollection<CourseCategory> CourseCategories { get; set; }
	}
}