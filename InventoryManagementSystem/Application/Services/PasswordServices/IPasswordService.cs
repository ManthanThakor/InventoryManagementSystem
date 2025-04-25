namespace Application.Services.PasswordServices
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
