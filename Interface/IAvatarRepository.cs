using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Interface {
	public interface IAvatarRepository {
		Avatar GetAvatar(Guid avatarId);
		Avatar? GetAvatarFromUser(ApplicationUser user);
		void LoadAvatarFromUser(ApplicationUser user);
		bool AvatarExists(Guid avatarId);
		bool CreateAvatar(Avatar avatar);
		bool DeleteAvatar(Avatar avatar);
		bool Save();
	}
}