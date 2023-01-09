using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Dto {
	public class ApplicationUserDto {
		public string Id { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public AvatarDto? Avatar { get; set; }
	}
}