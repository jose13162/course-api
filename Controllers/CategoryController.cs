using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using course_api.Dto;
using course_api.Interface;
using course_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace course_api.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class CategoryController : Controller {
		private readonly IMapper _mapper;
		private readonly ICategoryRepository _categoryRepository;

		public CategoryController(IMapper mapper, ICategoryRepository categoryRepository) {
			this._mapper = mapper;
			this._categoryRepository = categoryRepository;
		}

		[HttpGet]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public IActionResult GetCategories() {
			var categories = this._categoryRepository.GetCategories();
			var mappedCategories = this._mapper.Map<ICollection<CategoryDto>>(categories);

			return Ok(mappedCategories);
		}

		[HttpGet("{categoryId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public IActionResult GetCategory(Guid categoryId) {
			if (!this._categoryRepository.CategoryExists(categoryId)) {
				ModelState.AddModelError("", "The category does not exist");

				return NotFound(ModelState);
			}

			var category = this._categoryRepository.GetCategory(categoryId);
			var mappedCategory = this._mapper.Map<CategoryDto>(category);

			return Ok(mappedCategory);
		}

		[HttpPost]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult CreateCategory([FromBody] CategoryDto category) {
			var mappedCategory = this._mapper.Map<Category>(category);
			var existingCategory = this._categoryRepository.GetCategoryByName(category.Name);

			if (existingCategory != null) {
				ModelState.AddModelError("", "A category with this name already exists");

				return BadRequest(ModelState);
			}

			if (!this._categoryRepository.CreateCategory(mappedCategory)) {
				ModelState.AddModelError("", "Something went wrong creating the category");

				return BadRequest(ModelState);
			}

			return Ok();
		}

		[HttpPut]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult UpdateCategory([FromQuery] Guid categoryId, [FromBody] CategoryDto category) {
			if (categoryId != category.Id) {
				ModelState.AddModelError("", "The categoryId from query does not match the body id");

				return BadRequest(ModelState);
			}

			if (!this._categoryRepository.CategoryExists(categoryId)) {
				ModelState.AddModelError("", "The category does not exist");

				return NotFound(ModelState);
			}

			var existingCategory = this._categoryRepository.GetCategoryByName(category.Name);

			if (existingCategory != null) {
				ModelState.AddModelError("", "A category with this name already exists");

				return BadRequest(ModelState);
			}

			var mappedCategory = this._mapper.Map<Category>(category);

			if (!this._categoryRepository.UpdateCategory(mappedCategory)) {
				ModelState.AddModelError("", "Something went wrong updating the category");
			}

			return Ok();
		}

		[HttpDelete("{categoryId}")]
		[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
		public IActionResult DeleteCategory(Guid categoryId) {
			if (!this._categoryRepository.CategoryExists(categoryId)) {
				ModelState.AddModelError("", "The category does not exist");

				return NotFound(ModelState);
			}

			var category = this._categoryRepository.GetCategory(categoryId);

			if (!this._categoryRepository.DeleteCategory(category)) {
				ModelState.AddModelError("", "Something went wrong deleting the category");
			}

			return Ok();
		}
	}
}