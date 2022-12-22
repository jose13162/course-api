using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course_api.Interface {
	public interface IFileUploader {
		(string, string) Upload(IFormFile file);
		void Delete(string fileName);
	}
}