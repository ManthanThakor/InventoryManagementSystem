using Application.Services.AdminService;
using Domain.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            _logger.LogInformation("Fetching admin dashboard stats...");

            try
            {
                var stats = await _adminService.GetDashboardStats();
                _logger.LogInformation("Successfully retrieved dashboard stats.");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching dashboard stats.");
                return HandleException(ex);
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users...");

            try
            {
                var users = await _adminService.GetAllUsers();
                _logger.LogInformation("Retrieved {Count} users.", users.Count());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching users.");
                return HandleException(ex);
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            try
            {
                var user = await _adminService.GetUserById(id);
                _logger.LogInformation("Successfully retrieved user: {UserId}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user with ID: {UserId}", id);
                return HandleException(ex);
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

            try
            {
                var result = await _adminService.DeleteUser(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
                    return Ok(new { success = true });
                }
                else
                {
                    _logger.LogWarning("Failed to delete user with ID: {UserId}", id);
                    return BadRequest(new { success = false, message = "Failed to delete user." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user with ID: {UserId}", id);
                return HandleException(ex);
            }
        }

        [HttpGet("user-types")]
        public async Task<IActionResult> GetAllUserTypes()
        {
            _logger.LogInformation("Fetching all user types...");

            try
            {
                var userTypes = await _adminService.GetAllUserTypes();
                _logger.LogInformation("Retrieved {Count} user types.", userTypes.Count());
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user types.");
                return HandleException(ex);
            }
        }

        [HttpGet("user-types/{id}")]
        public async Task<IActionResult> GetUserTypeById(Guid id)
        {
            _logger.LogInformation("Fetching user type with ID: {UserTypeId}", id);

            try
            {
                var userType = await _adminService.GetUserTypeById(id);
                _logger.LogInformation("Successfully retrieved user type: {UserTypeId}", id);
                return Ok(userType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user type with ID: {UserTypeId}", id);
                return HandleException(ex);
            }
        }

        [HttpPost("user-types")]
        public async Task<IActionResult> CreateUserType([FromBody] UserTypeCreateViewModel model)
        {
            _logger.LogInformation("Creating new user type...");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for creating user type.");
                    return BadRequest(ModelState);
                }

                var userType = await _adminService.CreateUserType(model);
                _logger.LogInformation("Successfully created user type with ID: {UserTypeId}", userType.Id);
                return CreatedAtAction(nameof(GetUserTypeById), new { id = userType.Id }, userType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user type.");
                return HandleException(ex);
            }
        }

        [HttpPut("user-types/{id}")]
        public async Task<IActionResult> UpdateUserType(Guid id, [FromBody] UserTypeUpdateViewModel model)
        {
            _logger.LogInformation("Updating user type with ID: {UserTypeId}", id);

            try
            {
                if (id != model.Id)
                {
                    _logger.LogWarning("ID mismatch for user type update. Route ID: {RouteId}, Model ID: {ModelId}", id, model.Id);
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for updating user type.");
                    return BadRequest(ModelState);
                }

                var userType = await _adminService.UpdateUserType(model);
                _logger.LogInformation("Successfully updated user type with ID: {UserTypeId}", userType.Id);
                return Ok(userType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user type with ID: {UserTypeId}", id);
                return HandleException(ex);
            }
        }

        [HttpDelete("user-types/{id}")]
        public async Task<IActionResult> DeleteUserType(Guid id)
        {
            _logger.LogInformation("Attempting to delete user type with ID: {UserTypeId}", id);

            try
            {
                var result = await _adminService.DeleteUserType(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted user type with ID: {UserTypeId}", id);
                    return Ok(new { success = true });
                }
                else
                {
                    _logger.LogWarning("Failed to delete user type with ID: {UserTypeId}", id);
                    return BadRequest(new { success = false, message = "Failed to delete user type." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user type with ID: {UserTypeId}", id);
                return HandleException(ex);
            }
        }

        private IActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred in AdminController.");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
