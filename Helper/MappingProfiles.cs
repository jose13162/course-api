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
			CreateMap<Course, CourseDto>();
			CreateMap<CourseDto, Course>();
		}
	}
}