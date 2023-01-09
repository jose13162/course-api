using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course_api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace course_api.Data {
	public class DataContext : IdentityDbContext<ApplicationUser> {
		public DataContext(DbContextOptions options) : base(options) {
		}

		public DbSet<Course> Courses { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<Recording> Recordings { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<CourseCategory> CourseCategories { get; set; }
		public DbSet<Cover> Covers { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Avatar> Avatars { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Course>()
				.HasMany((course) => course.Lessons)
				.WithOne((lesson) => lesson.Course)
				.HasForeignKey((lesson) => lesson.CourseId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<Course>()
				.HasOne((course) => course.Cover)
				.WithOne((cover) => cover.Course)
				.HasForeignKey<Cover>((cover) => cover.CourseId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Lesson>()
				.HasOne((lesson) => lesson.Recording)
				.WithOne((recording) => recording.Lesson)
				.HasForeignKey<Recording>((recording) => recording.LessonId)
				.OnDelete(DeleteBehavior.Cascade);

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

			modelBuilder.Entity<ApplicationUser>()
				.HasOne((user) => user.Avatar)
				.WithOne((avatar) => avatar.User)
				.HasForeignKey<Avatar>((avatar) => avatar.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			base.OnModelCreating(modelBuilder);
		}
	}
}