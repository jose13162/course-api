using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Dto {
	public class CourseDto {
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public ICollection<CategoryDto>? Categories { get; set; }
		public CoverDto? Cover { get; set; }
	}
}