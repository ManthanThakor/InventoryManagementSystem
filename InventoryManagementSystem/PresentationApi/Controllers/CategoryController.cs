using Application.Services.CategoryServices;
using Domain.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                if (categories != null)
                {
                    return Ok(categories);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category != null)
                {
                    return Ok(category);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
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
                    var category = await _categoryService.CreateCategory(model);
                    if (category != null)
                    {
                        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
                    }
                    else
                    {
                        return BadRequest("Failed to create category.");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
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
                        var category = await _categoryService.UpdateCategory(model);
                        if (category != null)
                        {
                            return Ok(category);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    return BadRequest("ID mismatch");
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);
                if (result)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to delete category." });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories([FromQuery] string searchTerm)
        {
            try
            {
                var categories = await _categoryService.SearchCategories(searchTerm);
                if (categories != null && categories.Any())
                {
                    return Ok(categories);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
