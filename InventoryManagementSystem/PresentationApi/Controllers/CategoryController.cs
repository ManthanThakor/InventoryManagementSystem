using Application.Services.CategoryServices;
using Domain.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories.");
                var categories = await _categoryService.GetAllCategories();
                if (categories != null)
                {
                    _logger.LogInformation("Fetched {Count} categories.", categories.Count());
                    return Ok(categories);
                }
                else
                {
                    _logger.LogWarning("No categories found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories.");
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching category by ID: {Id}", id);
                var category = await _categoryService.GetCategoryById(id);
                if (category != null)
                {
                    return Ok(category);
                }
                else
                {
                    _logger.LogWarning("Category not found for ID: {Id}", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by ID: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Creating new category: {Name}", model.Name);
                    var category = await _categoryService.CreateCategory(model);
                    if (category != null)
                    {
                        _logger.LogInformation("Category created with ID: {Id}", category.Id);
                        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create category.");
                        return BadRequest("Failed to create category.");
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid category model state.");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryUpdateViewModel model)
        {
            try
            {
                if (id == model.Id)
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation("Updating category with ID: {Id}", id);
                        var category = await _categoryService.UpdateCategory(model);
                        if (category != null)
                        {
                            _logger.LogInformation("Category updated for ID: {Id}", id);
                            return Ok(category);
                        }
                        else
                        {
                            _logger.LogWarning("Category not found to update for ID: {Id}", id);
                            return NotFound();
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid model state for category update.");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    _logger.LogWarning("ID mismatch for category update: route ID = {RouteId}, model ID = {ModelId}", id, model.Id);
                    return BadRequest("ID mismatch");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting category with ID: {Id}", id);
                var result = await _categoryService.DeleteCategory(id);
                if (result)
                {
                    _logger.LogInformation("Category deleted successfully for ID: {Id}", id);
                    return Ok(new { success = true });
                }
                else
                {
                    _logger.LogWarning("Failed to delete category with ID: {Id}", id);
                    return BadRequest(new { success = false, message = "Failed to delete category." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories([FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching categories with term: {SearchTerm}", searchTerm);
                var categories = await _categoryService.SearchCategories(searchTerm);
                if (categories != null && categories.Any())
                {
                    _logger.LogInformation("Found {Count} categories for search term '{SearchTerm}'", categories.Count(), searchTerm);
                    return Ok(categories);
                }
                else
                {
                    _logger.LogWarning("No categories found for search term: {SearchTerm}", searchTerm);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching categories with term: {SearchTerm}", searchTerm);
                return HandleException(ex);
            }
        }

        private IActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex, "Internal server error.");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
