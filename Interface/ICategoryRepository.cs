using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Interface {
	public interface ICategoryRepository {
		ICollection<Category> GetCategories();
		Category GetCategory(Guid categoryId);
		Category GetCategoryByName(string name);
		bool CategoryExists(Guid categoryId);
		bool CreateCategory(Category category);
		bool UpdateCategory(Category category);
		bool DeleteCategory(Category category);
		bool Save();
	}
}