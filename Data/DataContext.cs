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

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Course>()
				.HasMany((course) => course.Lessons)
				.WithOne((lesson) => lesson.Course)
				.HasForeignKey((lesson) => lesson.CourseId);

			base.OnModelCreating(modelBuilder);
		}
	}
}