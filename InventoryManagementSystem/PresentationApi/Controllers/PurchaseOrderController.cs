using Application.Services.PurchaseOrderServices;
using Domain.ViewModels.Orders;
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
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderListViewModel>>> GetAllPurchaseOrders()
        {
            var purchaseOrders = await _purchaseOrderService.GetAllPurchaseOrders();
            return Ok(purchaseOrders);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<PurchaseOrderDetailViewModel>> GetPurchaseOrderById(Guid id)
        {
            try
            {
                var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderById(id);
                return Ok(purchaseOrder);
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

        [HttpPost]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<PurchaseOrderViewModel>> CreatePurchaseOrder(PurchaseOrderCreateViewModel model)
        {
            try
            {
                var purchaseOrder = await _purchaseOrderService.CreatePurchaseOrder(model);
                return CreatedAtAction(nameof(GetPurchaseOrderById), new { id = purchaseOrder.Id }, purchaseOrder);
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
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult> DeletePurchaseOrder(Guid id)
        {
            try
            {
                bool result = await _purchaseOrderService.DeletePurchaseOrder(id);
                if (result)
                {
                    return NoContent();
                }
                return NotFound($"Purchase Order with ID {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("supplier/{supplierId}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderListViewModel>>> GetPurchaseOrdersBySupplier(Guid supplierId)
        {
            try
            {
                var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersBySupplier(supplierId);
                return Ok(purchaseOrders);
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

        [HttpGet("search")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderListViewModel>>> SearchPurchaseOrders([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            try
            {
                var purchaseOrders = await _purchaseOrderService.SearchPurchaseOrders(searchTerm);
                return Ok(purchaseOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}