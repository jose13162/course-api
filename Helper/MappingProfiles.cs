using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using course_api.Dto;
using course_api.Models;

namespace course_api.Helper {
	public class MappingProfiles : Profile {
		public MappingProfiles() {
			CreateMap<Course, CourseDto>()
				.ForMember(
					(courseDto) => courseDto.Categories,
					(options) => options.MapFrom(
						(course) =>
							course.CourseCategories
								.Select((courseCategory) => courseCategory.Category)
								.ToList()
					)
				);
			CreateMap<CourseDto, Course>();
			CreateMap<Lesson, LessonDto>();
			CreateMap<LessonDto, Lesson>();
			CreateMap<Recording, RecordingDto>();
			CreateMap<RecordingDto, Recording>();
			CreateMap<Category, CategoryDto>()
				.ForMember(
					(categoryDto) => categoryDto.Courses,
					(options) => options.MapFrom(
						(category) =>
							category.CourseCategories
								.Select((courseCategory) => courseCategory.Course)
								.ToList()
					)
				); ;
			CreateMap<CategoryDto, Category>();
		}
	}
}