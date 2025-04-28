using Application.Services.CategoryServices;
using Domain.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PresentationApi.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetAll()
        {
            _logger.LogInformation("Getting all categories");
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDetailViewModel>> GetById(Guid id)
        {
            _logger.LogInformation("Getting category with ID: {CategoryId}", id);
            var category = await _categoryService.GetCategoryById(id);
            return Ok(category);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<ActionResult<CategoryViewModel>> Create([FromBody] CategoryCreateViewModel model)
        {
            _logger.LogInformation("Creating new category: {CategoryName}", model.Name);
            var createdCategory = await _categoryService.CreateCategory(model);
            _logger.LogInformation("Created category with ID: {CategoryId}", createdCategory.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<ActionResult<CategoryViewModel>> Update(Guid id, [FromBody] CategoryUpdateViewModel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning("ID mismatch in update request. Route ID: {RouteId}, Body ID: {BodyId}", id, model.Id);
                return BadRequest("ID mismatch");
            }

            _logger.LogInformation("Updating category with ID: {CategoryId}", id);
            var updatedCategory = await _categoryService.UpdateCategory(model);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
            var result = await _categoryService.DeleteCategory(id);

            if (!result)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term cannot be empty");
            }

            _logger.LogInformation("Searching categories with term: {SearchTerm}", term);
            var categories = await _categoryService.SearchCategories(term);
            return Ok(categories);
        }
    }
}