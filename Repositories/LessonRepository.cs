using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Data;
using course_api.Interface;
using course_api.Models;
using Microsoft.EntityFrameworkCore;

namespace course_api.Repositories {
	public class LessonRepository : ILessonRepository {
		private readonly DataContext _context;

		public LessonRepository(DataContext context) {
			this._context = context;
		}

		public ICollection<Lesson> GetLessons(Guid courseId) {
			return this._context.Lessons
				.Include((lesson) => lesson.Recording)
				.Where((lesson) => lesson.CourseId == courseId)
				.ToList();
		}

		public Lesson GetLesson(Guid lessonId) {
			return this._context.Lessons
			.Include((lesson) => lesson.Recording)
			.Where((lesson) => lesson.Id == lessonId)
			.FirstOrDefault();
		}

		public bool LessonExists(Guid lessonId) {
			return this._context.Lessons
				.Any((lesson) => lesson.Id == lessonId);
		}

		public bool CreateLesson(Lesson lesson) {
			this._context.Lessons.Add(lesson);

			return this.Save();
		}

		public bool UpdateLesson(Lesson lesson) {
			this._context.Lessons.Update(lesson);

			return this.Save();
		}

		public bool DeleteLesson(Lesson lesson) {
			this._context.Lessons.Remove(lesson);

			return this.Save();
		}

		public bool Save() {
			var affectedRows = this._context.SaveChanges();

			return affectedRows > 0;
		}
	}
}