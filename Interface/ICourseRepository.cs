using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Interface {
	public interface ICourseRepository {
		ICollection<Course> GetCourses();
		Course GetCourse(Guid courseId);
		bool CourseExists(Guid courseId);
		bool CreateCourse(Course course);
		bool UpdateCourse(Course course);
		bool DeleteCourse(Course course);
		CourseCategory GetCourseCategory(Course course, Guid categoryId);
		bool HasCategory(Course course, Guid categoryId);
		bool AddCategory(CourseCategory courseCategory);
		bool RemoveCategory(CourseCategory courseCategory);
		bool Save();
	}
} 