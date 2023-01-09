using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;

namespace course_api.Repositories {
	public class AvatarRepository : IAvatarRepository {
		private readonly DataContext _context;

		public AvatarRepository(DataContext context) {
			this._context = context;
		}

		public Avatar GetAvatar(Guid avatarId) {
			return this._context.Avatars
				.Where((avatar) => avatar.Id == avatarId)
				.FirstOrDefault();
		}

		public bool AvatarExists(Guid avatarId) {
			return this._context.Avatars.Any((avatar) => avatar.Id == avatarId);
		}

		public Avatar? GetAvatarFromUser(ApplicationUser user) {
			return this._context.Avatars
				.Where((avatar) => avatar.UserId == user.Id)
				.FirstOrDefault();
		}

		public void LoadAvatarFromUser(ApplicationUser user) {
			this._context.Entry(user)
				.Reference((user) => user.Avatar)
				.Load();
		}

		public bool CreateAvatar(Avatar avatar) {
			this._context.Avatars.Add(avatar);

			return this.Save();
		}

		public bool DeleteAvatar(Avatar avatar) {
			this._context.Avatars.Remove(avatar);

			return this.Save();
		}

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}


	}
}