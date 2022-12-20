using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;

namespace course_api.Interface {
	public interface ILessonRepository {
		ICollection<Lesson> GetLessons(Guid courseId);
		Lesson GetLesson(Guid lessonId);
		bool LessonExists(Guid lessonId);
		bool CreateLesson(Lesson lesson);
		bool UpdateLesson(Lesson lesson);
		bool DeleteLesson(Lesson lesson);
		bool Save();
	}
}