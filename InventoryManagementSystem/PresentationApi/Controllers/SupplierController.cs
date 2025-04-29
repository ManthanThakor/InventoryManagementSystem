using Application.Services.SupplierServices;
using Domain.ViewModels.Orders;
using Domain.ViewModels.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SupplierViewModel>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<SupplierDetailViewModel>> GetSupplierById(Guid id)
        {
            try
            {
                var supplier = await _supplierService.GetSupplierById(id);
                return Ok(supplier);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<SupplierViewModel>> UpdateSupplier(Guid id, SupplierUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedSupplier = await _supplierService.UpdateSupplier(model);
                return Ok(updatedSupplier);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> DeleteSupplier(Guid id)
        {
            try
            {
                bool result = await _supplierService.DeleteSupplier(id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Supplier with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{supplierId}/items")]
        public async Task<ActionResult<IEnumerable<SupplierItemViewModel>>> GetItemsBySupplier(Guid supplierId)
        {
            try
            {
                var items = await _supplierService.GetItemsBySupplier(supplierId);
                return Ok(items);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("items/{supplierItemId}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult> RemoveSupplierItem(Guid supplierItemId)
        {
            try
            {
                bool result = await _supplierService.RemoveSupplierItem(supplierItemId);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Supplier item with ID {supplierItemId} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SupplierViewModel>>> SearchSuppliers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }
            else
            {
                try
                {
                    var suppliers = await _supplierService.SearchSuppliers(searchTerm);
                    return Ok(suppliers);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}
