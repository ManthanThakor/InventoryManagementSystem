using Application.Services.CustomerServices;
using Domain.ViewModels.Customer;
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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<CustomerViewModel>>> GetAllCustomers()
        {
            _logger.LogInformation("Fetching all customers.");
            var customers = await _customerService.GetAllCustomers();
            _logger.LogInformation("Retrieved {Count} customers.", customers?.Count());
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<CustomerDetailViewModel>> GetCustomerById(Guid id)
        {
            _logger.LogInformation("Fetching customer with ID: {CustomerId}", id);
            try
            {
                var customer = await _customerService.GetCustomerById(id);
                return Ok(customer);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Customer with ID {CustomerId} not found.", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching customer with ID {CustomerId}.", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer(Guid id, CustomerUpdateViewModel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning("UpdateCustomer ID mismatch: route ID {RouteId}, model ID {ModelId}", id, model.Id);
                return BadRequest("ID mismatch");
            }

            _logger.LogInformation("Updating customer with ID: {CustomerId}", id);

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomer(model);
                return Ok(updatedCustomer);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Null argument while updating customer {CustomerId}", id);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Customer not found for update: {CustomerId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating customer {CustomerId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCustomer(Guid id)
        {
            _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);
            try
            {
                bool result = await _customerService.DeleteCustomer(id);
                if (result)
                {
                    _logger.LogInformation("Successfully deleted customer {CustomerId}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
                    return NotFound($"Customer with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting customer {CustomerId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{customerId}/items")]
        [Authorize(Roles = "Admin,Customer,Supplier")]
        public async Task<ActionResult<IEnumerable<CustomerItemViewModel>>> GetItemsByCustomer(Guid customerId)
        {
            _logger.LogInformation("Fetching items for customer {CustomerId}", customerId);
            try
            {
                var items = await _customerService.GetItemsByCustomer(customerId);
                return Ok(items);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No items found for customer {CustomerId}", customerId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching items for customer {CustomerId}", customerId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("items/{customerItemId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult> RemoveCustomerItem(Guid customerItemId)
        {
            _logger.LogInformation("Removing item {CustomerItemId} for customer", customerItemId);
            try
            {
                bool result = await _customerService.RemoveCustomerItem(customerItemId);
                if (result)
                {
                    _logger.LogInformation("Successfully removed item {CustomerItemId}", customerItemId);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Customer item with ID {CustomerItemId} not found", customerItemId);
                    return NotFound($"Customer item with ID {customerItemId} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing customer item {CustomerItemId}", customerItemId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Supplier")]
        public async Task<ActionResult<IEnumerable<CustomerViewModel>>> SearchCustomers([FromQuery] string searchTerm)
        {
            _logger.LogInformation("Searching customers with term: {SearchTerm}", searchTerm);

            if (string.IsNullOrEmpty(searchTerm))
            {
                _logger.LogWarning("Search term is empty.");
                return BadRequest("Search term is required");
            }

            try
            {
                var customers = await _customerService.SearchCustomers(searchTerm);
                _logger.LogInformation("Found {Count} customers matching term: {SearchTerm}", customers?.Count(), searchTerm);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
