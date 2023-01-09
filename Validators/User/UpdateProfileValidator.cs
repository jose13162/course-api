using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Validators.User {
	public class UpdateProfileValidator {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		[MinLength(6)]
		[MaxLength(32)]
		public string Username { get; set; }
	}
}