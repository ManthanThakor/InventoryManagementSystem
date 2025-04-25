using Application.Services.PasswordServices;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _hasher = new();

        private readonly object _passwordHasherUser = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(_passwordHasherUser, password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(_passwordHasherUser, passwordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
