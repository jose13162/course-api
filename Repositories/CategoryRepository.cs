using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;
using Microsoft.EntityFrameworkCore;

namespace course_api.Repositories {
	public class CategoryRepository : ICategoryRepository {
		private readonly DataContext _context;

		public CategoryRepository(DataContext context) {
			this._context = context;
		}

		public ICollection<Category> GetCategories() {
			return this._context.Categories.ToList();
		}

		public Category GetCategory(Guid categoryId) {
			var category = this._context.Categories
				.Include((category) => category.CourseCategories)
				.Where((category) => category.Id == categoryId)
				.FirstOrDefault();

			category.CourseCategories.ToList().ForEach((courseCategory) => {
				this._context.Entry(courseCategory)
					.Reference((courseCategory) => courseCategory.Course)
					.Load();
			});

			return category;
		}

		public Category GetCategoryByName(string name) {
			var capitalizedName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
			return this._context.Categories
				.Where((category) => category.Name == capitalizedName)
				.FirstOrDefault();
		}

		public bool CategoryExists(Guid categoryId) {
			return this._context.Categories
				.Any((category) => category.Id == categoryId);
		}

		public bool CreateCategory(Category category) {
			category.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(category.Name.ToLower());
			this._context.Categories.Add(category);

			return this.Save();
		}

		public bool UpdateCategory(Category category) {
			category.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(category.Name.ToLower());
			this._context.Categories.Update(category);

			return this.Save();
		}

		public bool DeleteCategory(Category category) {
			this._context.Categories.Remove(category);

			return this.Save();
		}

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}
	}
}