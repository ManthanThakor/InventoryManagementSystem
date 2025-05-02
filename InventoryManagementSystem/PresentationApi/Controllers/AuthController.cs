using Application.Services.AuthServices;
using Domain.Models;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.Customer;
using Domain.ViewModels.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            _logger.LogInformation("Login attempt for user: {Username}", model.Username);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Login model state invalid for user: {Username}", model.Username);
                    return BadRequest(ModelState);
                }

                var response = await _authService.Login(model);
                _logger.LogInformation("Login successful for user: {Username}", model.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user: {Username}", model.Username);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            _logger.LogInformation("Registering user: {Username}", model.Username);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Register model state invalid for user: {Username}", model.Username);
                    return BadRequest(ModelState);
                }

                var response = await _authService.Register(model);
                _logger.LogInformation("User registered successfully: {Username}", model.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User registration failed: {Username}", model.Username);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register/customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterViewModel model)
        {
            _logger.LogInformation("Registering customer: {Username}", model.Username);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Customer registration model invalid: {Username}", model.Username);
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterCustomer(model);
                _logger.LogInformation("Customer registered: {Username}", model.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer registration failed: {Username}", model.Username);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register/supplier")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSupplier([FromBody] SupplierRegisterViewModel model)
        {
            _logger.LogInformation("Registering supplier: {Username}", model.Username);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Supplier registration model invalid: {Username}", model.Username);
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterSupplier(model);
                _logger.LogInformation("Supplier registered: {Username}", model.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier registration failed: {Username}", model.Username);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            _logger.LogInformation("Fetching user profile");

            try
            {
                Guid userId = GetCurrentUserId();

                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthorized attempt to access profile.");
                    return Unauthorized();
                }

                var profile = await _authService.GetUserProfile(userId);
                _logger.LogInformation("User profile fetched for userId: {UserId}", userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profile");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            _logger.LogInformation("Password change attempt");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Change password model state invalid");
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();

                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthorized attempt to change password.");
                    return Unauthorized();
                }

                var result = await _authService.ChangePassword(userId, model);
                _logger.LogInformation("Password changed for userId: {UserId}", userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change failed");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogWarning("Failed to parse current user ID");
                return Guid.Empty;
            }

            return userId;
        }
    }
}
