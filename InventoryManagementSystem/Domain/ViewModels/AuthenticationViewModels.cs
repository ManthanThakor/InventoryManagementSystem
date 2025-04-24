using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace YourNamespace.Application.ViewModels
    {
        public class RegisterViewModel
        {
            [Required(ErrorMessage = "Full Name is required")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
            [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, dots, underscores, and hyphens")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Confirm Password is required")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "User Type is required")]
            public int UserTypeId { get; set; }
        }

        public class LoginViewModel
        {
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = string.Empty;
        }

        public class JwtResponseViewModel
        {
            public string Token { get; set; } = string.Empty;
            public DateTime Expiration { get; set; }
            public string Username { get; set; } = string.Empty;
            public string UserType { get; set; } = string.Empty;
            public Guid UserId { get; set; }
        }

        public class UserProfileViewModel
        {
            public Guid Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string UserType { get; set; } = string.Empty;
        }

        public class ChangePasswordViewModel
        {
            [Required(ErrorMessage = "Current password is required")]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "New password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Confirm password is required")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}