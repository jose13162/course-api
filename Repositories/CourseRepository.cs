using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;

namespace course_api.Repositories {
	public class CourseRepository : ICourseRepository {
		private readonly DataContext _context;

		public CourseRepository(DataContext context) {
			this._context = context;
		}

		public ICollection<Course> GetCourses() {
			return this._context.Courses.ToList();
		}

		public Course GetCourse(Guid courseId) {
			return this._context.Courses
				.Where((course) => course.Id == courseId)
				.FirstOrDefault();
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

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}
	}
}