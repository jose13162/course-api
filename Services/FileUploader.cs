using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Interface;

namespace course_api.Services {
	public class FileUploader : IFileUploader {
		private readonly IWebHostEnvironment _environment;
		private readonly string _uploadsPath;

		public FileUploader(IWebHostEnvironment environment) {
			this._environment = environment;

			var applicationPath = this._environment.ContentRootPath;
			var uploadsPath = Path.Combine(applicationPath, "uploads");

			this._uploadsPath = uploadsPath;
		}
		public (string, string) Upload(IFormFile file) {
			var fileNameParts = file.FileName.Split('.');
			var extensionName = fileNameParts.LastOrDefault();
			var fileNameGuid = Guid.NewGuid().ToString();
			var fileName = $"{fileNameGuid}.{extensionName}";
			var path = Path.Combine(this._uploadsPath, fileName);

			using (Stream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				file.CopyTo(fileStream);
			}

			return (fileName, path);
		}

		public void Delete(string fileName) {
			var path = Path.Combine(this._uploadsPath, fileName);

			File.Delete(path);
		}
	}
}
