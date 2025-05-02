using Application.Services.SalesOrderServices;
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
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly ILogger<SalesOrderController> _logger;

        public SalesOrderController(ISalesOrderService salesOrderService, ILogger<SalesOrderController> logger)
        {
            _salesOrderService = salesOrderService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SalesOrderListViewModel>>> GetAllSalesOrders()
        {
            _logger.LogInformation("Getting all sales orders.");
            var salesOrders = await _salesOrderService.GetAllSalesOrders();
            return Ok(salesOrders);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<SalesOrderDetailViewModel>> GetSalesOrderById(Guid id)
        {
            _logger.LogInformation("Getting sales order by ID: {SalesOrderId}", id);
            try
            {
                var salesOrder = await _salesOrderService.GetSalesOrderById(id);
                return Ok(salesOrder);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Sales order not found with ID: {SalesOrderId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales order with ID: {SalesOrderId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<SalesOrderViewModel>> CreateSalesOrder(SalesOrderCreateViewModel model)
        {
            _logger.LogInformation("Creating a new sales order.");
            try
            {
                var salesOrder = await _salesOrderService.CreateSalesOrder(model);
                _logger.LogInformation("Sales order created with ID: {SalesOrderId}", salesOrder.Id);
                return CreatedAtAction(nameof(GetSalesOrderById), new { id = salesOrder.Id }, salesOrder);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid input while creating sales order.");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Related entity not found while creating sales order.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sales order.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult> DeleteSalesOrder(Guid id)
        {
            _logger.LogInformation("Deleting sales order with ID: {SalesOrderId}", id);
            try
            {
                bool result = await _salesOrderService.DeleteSalesOrder(id);
                if (result)
                {
                    _logger.LogInformation("Sales order deleted with ID: {SalesOrderId}", id);
                    return NoContent();
                }

                _logger.LogWarning("Sales order not found for deletion with ID: {SalesOrderId}", id);
                return NotFound($"Sales Order with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sales order with ID: {SalesOrderId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SalesOrderListViewModel>>> GetSalesOrdersByCustomer(Guid customerId)
        {
            _logger.LogInformation("Getting sales orders for customer ID: {CustomerId}", customerId);
            try
            {
                var salesOrders = await _salesOrderService.GetSalesOrdersByCustomer(customerId);
                return Ok(salesOrders);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Customer not found with ID: {CustomerId}", customerId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales orders for customer ID: {CustomerId}", customerId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SalesOrderListViewModel>>> SearchSalesOrders([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                _logger.LogWarning("Search term is missing.");
                return BadRequest("Search term is required");
            }

            _logger.LogInformation("Searching sales orders with term: {SearchTerm}", searchTerm);
            try
            {
                var salesOrders = await _salesOrderService.SearchSalesOrders(searchTerm);
                return Ok(salesOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sales orders with term: {SearchTerm}", searchTerm);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
