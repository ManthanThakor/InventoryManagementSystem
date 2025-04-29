using Application.Services.AdminService;
using Domain.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var stats = await _adminService.GetDashboardStats();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _adminService.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var result = await _adminService.DeleteUser(id);

                if (result)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to delete user." });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("user-types")]
        public async Task<IActionResult> GetAllUserTypes()
        {
            try
            {
                var userTypes = await _adminService.GetAllUserTypes();
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("user-types/{id}")]
        public async Task<IActionResult> GetUserTypeById(Guid id)
        {
            try
            {
                var userType = await _adminService.GetUserTypeById(id);
                return Ok(userType);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("user-types")]
        public async Task<IActionResult> CreateUserType([FromBody] UserTypeCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var userType = await _adminService.CreateUserType(model);
                    return CreatedAtAction(nameof(GetUserTypeById), new { id = userType.Id }, userType);
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("user-types/{id}")]
        public async Task<IActionResult> UpdateUserType(Guid id, [FromBody] UserTypeUpdateViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest("ID mismatch");
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        var userType = await _adminService.UpdateUserType(model);
                        return Ok(userType);
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("user-types/{id}")]
        public async Task<IActionResult> DeleteUserType(Guid id)
        {
            try
            {
                var result = await _adminService.DeleteUserType(id);

                if (result)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to delete user type." });
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
