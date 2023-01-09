using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Models {
	public class Avatar {
		public Guid Id { get; set; }
		public string UserId { get; set; }
		public string FileName { get; set; }
		public string Url { get; set; }
		public virtual ApplicationUser User { get; set; }
	}
}