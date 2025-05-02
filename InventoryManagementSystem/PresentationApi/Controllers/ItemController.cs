using Application.Services.ItemServices;
using Domain.ViewModels.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                _logger.LogInformation("Fetching all items.");
                IEnumerable<ItemViewModel> items = await _itemService.GetAllItems();
                _logger.LogInformation("Successfully fetched {Count} items.", items.Count());
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all items.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching item with ID: {ItemId}", id);
                ItemDetailViewModel item = await _itemService.GetItemById(id);

                if (item != null)
                {
                    _logger.LogInformation("Item with ID: {ItemId} found.", id);
                    return Ok(item);
                }
                else
                {
                    _logger.LogWarning("Item with ID: {ItemId} not found.", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching item with ID: {ItemId}", id);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("CreateItem")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while creating item.");
                    return BadRequest(ModelState);
                }

                ItemViewModel item = await _itemService.CreateItem(model);
                _logger.LogInformation("Item created with ID: {ItemId}", item.Id);
                return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating item.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    _logger.LogWarning("ID mismatch: route ID {RouteId} does not match model ID {ModelId}.", id, model.Id);
                    return BadRequest(new { success = false, message = "ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while updating item.");
                    return BadRequest(ModelState);
                }

                ItemViewModel item = await _itemService.UpdateItem(model);
                _logger.LogInformation("Item updated with ID: {ItemId}", item.Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating item with ID: {ItemId}", id);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete item with ID: {ItemId}", id);
                bool result = await _itemService.DeleteItem(id);

                if (result)
                {
                    _logger.LogInformation("Item with ID: {ItemId} deleted successfully.", id);
                    return Ok(new { success = true });
                }
                else
                {
                    _logger.LogWarning("Item with ID: {ItemId} not found for deletion.", id);
                    return NotFound(new { success = false, message = "Item not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting item with ID: {ItemId}", id);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetItemsByCategory(Guid categoryId)
        {
            try
            {
                _logger.LogInformation("Fetching items for category ID: {CategoryId}", categoryId);
                IEnumerable<ItemViewModel> items = await _itemService.GetItemsByCategory(categoryId);
                _logger.LogInformation("{Count} items found for category ID: {CategoryId}", items.Count(), categoryId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching items by category ID: {CategoryId}", categoryId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogWarning("Search attempted with empty search term.");
                    return BadRequest(new { success = false, message = "Search term cannot be empty." });
                }

                _logger.LogInformation("Searching items with term: {SearchTerm}", searchTerm);
                IEnumerable<ItemViewModel> items = await _itemService.SearchItems(searchTerm);
                _logger.LogInformation("Found {Count} items matching search term: {SearchTerm}", items.Count(), searchTerm);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while searching items with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
