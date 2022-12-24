using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;
using Microsoft.EntityFrameworkCore;

namespace course_api.Repositories {
	public class CourseRepository : ICourseRepository {
		private readonly DataContext _context;

		public CourseRepository(DataContext context) {
			this._context = context;
		}

		public ICollection<Course> GetCourses() {
			var courses = this._context.Courses
				.Include((course) => course.CourseCategories)
				.ToList();

			courses.ForEach((course) => {
				course.CourseCategories.ToList().ForEach((courseCategory) => {
					this._context.Entry(courseCategory)
						.Reference((courseCategory) => courseCategory.Category)
						.Load();
				});
			});

			return courses;
		}

		public Course GetCourse(Guid courseId) {
			var course = this._context.Courses
				.Include((course) => course.CourseCategories)
				.Where((course) => course.Id == courseId)
				.FirstOrDefault();

			course.CourseCategories.ToList().ForEach((courseCategory) => {
				this._context.Entry(courseCategory)
					.Reference((courseCategory) => courseCategory.Category)
					.Load();
			});

			return course;
		}

		public bool CourseExists(Guid courseId) {
			return this._context.Courses
				.Any((course) => course.Id == courseId);
		}
		public bool CreateCourse(Course course) {
			this._context.Courses.Add(course);

			return this.Save();
		}

		public bool UpdateCourse(Course course) {
			this._context.Courses.Update(course);

			return this.Save();
		}

		public bool DeleteCourse(Course course) {
			this._context.Courses.Remove(course);

			return this.Save();
		}

		public bool HasCategory(Course course, Guid categoryId) {
			return this._context.CourseCategories
				.Any((courseCategory) => courseCategory.CourseId == course.Id & courseCategory.CategoryId == categoryId);
		}

		public bool AddCategory(CourseCategory courseCategory) {
			this._context.CourseCategories.Add(courseCategory);

			return this.Save();
		}

		public bool RemoveCategory(CourseCategory courseCategory) {
			this._context.CourseCategories.Remove(courseCategory);

			return this.Save();
		}

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}

		public CourseCategory GetCourseCategory(Course course, Guid categoryId) {
			return this._context.CourseCategories
				.Where((courseCategory) => courseCategory.CourseId == course.Id & courseCategory.CategoryId == categoryId)
				.FirstOrDefault();
		}
	}
}