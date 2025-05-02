using Application.Services.PurchaseOrderServices;
using Domain.ViewModels.Orders;
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
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, ILogger<PurchaseOrderController> logger)
        {
            _purchaseOrderService = purchaseOrderService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderListViewModel>>> GetAllPurchaseOrders()
        {
            _logger.LogInformation("Getting all purchase orders.");
            var purchaseOrders = await _purchaseOrderService.GetAllPurchaseOrders();
            return Ok(purchaseOrders);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<PurchaseOrderDetailViewModel>> GetPurchaseOrderById(Guid id)
        {
            _logger.LogInformation("Getting purchase order by ID: {Id}", id);
            try
            {
                var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderById(id);
                return Ok(purchaseOrder);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Purchase order not found for ID: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting purchase order by ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<PurchaseOrderViewModel>> CreatePurchaseOrder(PurchaseOrderCreateViewModel model)
        {
            _logger.LogInformation("Creating a new purchase order.");
            try
            {
                var purchaseOrder = await _purchaseOrderService.CreatePurchaseOrder(model);
                _logger.LogInformation("Successfully created purchase order with ID: {Id}", purchaseOrder.Id);
                return CreatedAtAction(nameof(GetPurchaseOrderById), new { id = purchaseOrder.Id }, purchaseOrder);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid input while creating purchase order.");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Related resource not found during purchase order creation.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a purchase order.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult> DeletePurchaseOrder(Guid id)
        {
            _logger.LogInformation("Deleting purchase order with ID: {Id}", id);
            try
            {
                bool result = await _purchaseOrderService.DeletePurchaseOrder(id);
                if (result)
                {
                    _logger.LogInformation("Successfully deleted purchase order with ID: {Id}", id);
                    return NoContent();
                }

                _logger.LogWarning("Purchase order with ID {Id} not found for deletion.", id);
                return NotFound($"Purchase Order with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting purchase order with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("supplier/{supplierId}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderListViewModel>>> GetPurchaseOrdersBySupplier(Guid supplierId)
        {
            _logger.LogInformation("Getting purchase orders for supplier ID: {SupplierId}", supplierId);
            try
            {
                var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersBySupplier(supplierId);
                return Ok(purchaseOrders);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No purchase orders found for supplier ID: {SupplierId}", supplierId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving purchase orders for supplier ID: {SupplierId}", supplierId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderListViewModel>>> SearchPurchaseOrders([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                _logger.LogWarning("Search term was empty.");
                return BadRequest("Search term is required");
            }

            _logger.LogInformation("Searching purchase orders with term: {SearchTerm}", searchTerm);
            try
            {
                var purchaseOrders = await _purchaseOrderService.SearchPurchaseOrders(searchTerm);
                return Ok(purchaseOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching purchase orders with term: {SearchTerm}", searchTerm);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
