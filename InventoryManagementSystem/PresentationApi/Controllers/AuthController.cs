using Application.Services.AuthServices;
using Domain.Models;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.Customer;
using Domain.ViewModels.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.Login(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.Register(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register/customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterCustomer(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register/supplier")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSupplier([FromBody] SupplierRegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterSupplier(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        //[Authorize]
        //[HttpGet("profile")]
        //public async Task<IActionResult> GetUserProfile()
        //{
        //    try
        //    {
        //        Guid userId = GetCurrentUserId();

        //        if (userId == Guid.Empty)
        //        {
        //            return Unauthorized();
        //        }

        //        var profile = await _authService.GetUserProfile(userId);
        //        return Ok(profile);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = ex.Message });
        //    }
        //}

        //[Authorize]
        //[HttpPost("change-password")]
        //public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var userId = GetCurrentUserId();

        //        if (userId == Guid.Empty)
        //        {
        //            return Unauthorized();
        //        }

        //        var result = await _authService.ChangePassword(userId, model);
        //        return Ok(new { success = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = ex.Message });
        //    }
        //}

        //private Guid GetCurrentUserId()
        //{
        //    Guid userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        //    return Ok(new { UserId = userId });

        //}
    }
}
