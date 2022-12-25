using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Interface {
	public interface ICoverRepository {
		Cover GetCover(Guid coverId);
		bool CoverExists(Guid coverId);
		bool CreateCover(Cover cover);
		bool DeleteCover(Cover cover);
		bool Save();
	}
}