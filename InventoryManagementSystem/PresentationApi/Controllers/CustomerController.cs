using Application.Services.CustomerServices;
using Domain.ViewModels.Customer;
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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<IEnumerable<CustomerViewModel>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomers();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSupplier")]
        public async Task<ActionResult<CustomerDetailViewModel>> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(id);
                return Ok(customer);
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
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer(Guid id, CustomerUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomer(model);
                return Ok(updatedCustomer);
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                bool result = await _customerService.DeleteCustomer(id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Customer with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{customerId}/items")]
        [Authorize(Roles = "Admin,Customer,Supplier")]
        public async Task<ActionResult<IEnumerable<CustomerItemViewModel>>> GetItemsByCustomer(Guid customerId)
        {
            try
            {
                var items = await _customerService.GetItemsByCustomer(customerId);
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

        [HttpPost("items")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<CustomerItemViewModel>> AddCustomerItem(CustomerItemCreateViewModel model)
        {
            try
            {
                var customerItem = await _customerService.AddCustomerItem(model);
                return CreatedAtAction(nameof(GetItemsByCustomer), new { customerId = model.CustomerId }, customerItem);
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


        [HttpDelete("items/{customerItemId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult> RemoveCustomerItem(Guid customerItemId)
        {
            try
            {
                bool result = await _customerService.RemoveCustomerItem(customerItemId);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Customer item with ID {customerItemId} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("search")]
        [Authorize(Roles = "Admin,Supplier")]
        public async Task<ActionResult<IEnumerable<CustomerViewModel>>> SearchCustomers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }
            else
            {
                try
                {
                    var customers = await _customerService.SearchCustomers(searchTerm);
                    return Ok(customers);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}