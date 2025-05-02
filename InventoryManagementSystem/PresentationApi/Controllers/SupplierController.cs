using Application.Services.SupplierServices;
using Domain.ViewModels.Orders;
using Domain.ViewModels.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PresentationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierService supplierService, ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SupplierViewModel>>> GetAllSuppliers()
        {
            _logger.LogInformation("Fetching all suppliers");
            var suppliers = await _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<SupplierDetailViewModel>> GetSupplierById(Guid id)
        {
            _logger.LogInformation("Fetching supplier with ID {SupplierId}", id);
            try
            {
                var supplier = await _supplierService.GetSupplierById(id);
                return Ok(supplier);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier not found: {SupplierId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier with ID {SupplierId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<SupplierViewModel>> UpdateSupplier(Guid id, SupplierUpdateViewModel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning("ID mismatch during supplier update. Route ID: {Id}, Model ID: {ModelId}", id, model.Id);
                return BadRequest("ID mismatch");
            }

            _logger.LogInformation("Updating supplier with ID {SupplierId}", id);

            try
            {
                var updatedSupplier = await _supplierService.UpdateSupplier(model);
                return Ok(updatedSupplier);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid data for updating supplier");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier not found during update: {SupplierId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier with ID {SupplierId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> DeleteSupplier(Guid id)
        {
            _logger.LogInformation("Deleting supplier with ID {SupplierId}", id);

            try
            {
                bool result = await _supplierService.DeleteSupplier(id);
                if (result)
                {
                    _logger.LogInformation("Successfully deleted supplier with ID {SupplierId}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Supplier with ID {SupplierId} not found for deletion", id);
                    return NotFound($"Supplier with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier with ID {SupplierId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{supplierId}/items")]
        public async Task<ActionResult<IEnumerable<SupplierItemViewModel>>> GetItemsBySupplier(Guid supplierId)
        {
            _logger.LogInformation("Fetching items for supplier with ID {SupplierId}", supplierId);

            try
            {
                var items = await _supplierService.GetItemsBySupplier(supplierId);
                return Ok(items);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier with ID {SupplierId} not found", supplierId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching items for supplier with ID {SupplierId}", supplierId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("items/{supplierItemId}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult> RemoveSupplierItem(Guid supplierItemId)
        {
            _logger.LogInformation("Removing supplier item with ID {SupplierItemId}", supplierItemId);

            try
            {
                bool result = await _supplierService.RemoveSupplierItem(supplierItemId);
                if (result)
                {
                    _logger.LogInformation("Successfully removed supplier item with ID {SupplierItemId}", supplierItemId);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Supplier item with ID {SupplierItemId} not found", supplierItemId);
                    return NotFound($"Supplier item with ID {supplierItemId} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing supplier item with ID {SupplierItemId}", supplierItemId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SupplierViewModel>>> SearchSuppliers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                _logger.LogWarning("Search term is null or empty");
                return BadRequest("Search term is required");
            }

            _logger.LogInformation("Searching suppliers with term: {SearchTerm}", searchTerm);

            try
            {
                var suppliers = await _supplierService.SearchSuppliers(searchTerm);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching suppliers with term: {SearchTerm}", searchTerm);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
