using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Validators.User {
	public class ChangePasswordValidator {
		[Required]
		public string Token { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }
	}
}