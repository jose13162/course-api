using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Dto {
	public class CategoryDto {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public ICollection<CourseDto>? Courses { get; set; }
	}
}