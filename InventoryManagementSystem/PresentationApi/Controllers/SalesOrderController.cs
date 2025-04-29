using Application.Services.SalesOrderServices;
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
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;

        public SalesOrderController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SalesOrderListViewModel>>> GetAllSalesOrders()
        {
            var salesOrders = await _salesOrderService.GetAllSalesOrders();
            return Ok(salesOrders);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<SalesOrderDetailViewModel>> GetSalesOrderById(Guid id)
        {
            try
            {
                var salesOrder = await _salesOrderService.GetSalesOrderById(id);
                return Ok(salesOrder);
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
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<SalesOrderViewModel>> CreateSalesOrder(SalesOrderCreateViewModel model)
        {
            try
            {
                var salesOrder = await _salesOrderService.CreateSalesOrder(model);
                return CreatedAtAction(nameof(GetSalesOrderById), new { id = salesOrder.Id }, salesOrder);
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
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult> DeleteSalesOrder(Guid id)
        {
            try
            {
                bool result = await _salesOrderService.DeleteSalesOrder(id);
                if (result)
                {
                    return NoContent();
                }
                return NotFound($"Sales Order with ID {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SalesOrderListViewModel>>> GetSalesOrdersByCustomer(Guid customerId)
        {
            try
            {
                var salesOrders = await _salesOrderService.GetSalesOrdersByCustomer(customerId);
                return Ok(salesOrders);
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
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<IEnumerable<SalesOrderListViewModel>>> SearchSalesOrders([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            try
            {
                var salesOrders = await _salesOrderService.SearchSalesOrders(searchTerm);
                return Ok(salesOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}