using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;
using Microsoft.EntityFrameworkCore;

namespace course_api.Data {
	public class DataContext : DbContext {
		public DataContext(DbContextOptions options) : base(options) {
		}

		public DbSet<Course> Courses { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<Recording> Recordings { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<CourseCategory> CourseCategories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Course>()
				.HasMany((course) => course.Lessons)
				.WithOne((lesson) => lesson.Course)
				.HasForeignKey((lesson) => lesson.CourseId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Lesson>()
				.HasOne((lesson) => lesson.Recording)
				.WithOne((recording) => recording.Lesson)
				.HasForeignKey<Recording>((recording) => recording.LessonId);
				
			modelBuilder.Entity<CourseCategory>()
				.HasKey((courseCategory) => new { courseCategory.CourseId, courseCategory.CategoryId });
			modelBuilder.Entity<CourseCategory>()
				.HasOne((courseCategory) => courseCategory.Course)
				.WithMany((course) => course.CourseCategories)
				.HasForeignKey((courseCategory) => courseCategory.CourseId);
			modelBuilder.Entity<CourseCategory>()
				.HasOne((courseCategory) => courseCategory.Category)
				.WithMany((category) => category.CourseCategories)
				.HasForeignKey((courseCategory) => courseCategory.CategoryId);

			base.OnModelCreating(modelBuilder);
		}
	}
}