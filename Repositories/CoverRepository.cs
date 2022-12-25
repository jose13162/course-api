using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;
using Microsoft.EntityFrameworkCore;

namespace course_api.Repositories {
	public class CoverRepository : ICoverRepository {
		private readonly DataContext _context;

		public CoverRepository(DataContext context) {
			this._context = context;
		}

		public Cover GetCover(Guid coverId) {
			return this._context.Covers
				.Include((cover) => cover.Course)
				.Where((cover) => cover.Id == coverId)
				.FirstOrDefault();
		}

		public bool CoverExists(Guid coverId) {
			return this._context.Covers.Any((cover) => cover.Id == coverId);
		}

		public bool CreateCover(Cover cover) {
			this._context.Covers.Add(cover);

			return this.Save();
		}

		public bool DeleteCover(Cover cover) {
			this._context.Covers.Remove(cover);

			return this.Save();
		}

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}
	}
}