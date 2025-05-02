using Application.Services.SupportServices;
using Domain.ViewModels.SupportViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerSupportController : ControllerBase
    {
        private readonly ICustomerSupportService _supportService;

        public CustomerSupportController(ICustomerSupportService supportService)
        {
            _supportService = supportService;
        }

        [HttpPost("message")]
        public async Task<IActionResult> SubmitSupportMessage([FromBody] SupportMessageCreateViewModel messageViewModel)
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token");
            }

            // Make sure the user is submitting their own message
            messageViewModel.UserId = userId;

            try
            {
                var result = await _supportService.SaveSupportMessage(messageViewModel);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetSupportHistory()
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token");
            }

            try
            {
                var history = await _supportService.GetUserSupportHistory(userId);
                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingSupportMessages()
        {
            try
            {
                var pendingMessages = await _supportService.GetAllPendingSupportMessages();
                return Ok(pendingMessages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("respond")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RespondToSupportMessage([FromBody] SupportResponseViewModel responseViewModel)
        {
            // Get admin ID from claims
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized("Admin ID not found in token");
            }

            responseViewModel.AdminId = adminId;

            try
            {
                var result = await _supportService.RespondToSupportMessage(responseViewModel);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}