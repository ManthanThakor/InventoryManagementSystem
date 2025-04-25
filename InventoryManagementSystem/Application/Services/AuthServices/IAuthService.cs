using Domain.ViewModels.Authentication;
using Domain.ViewModels.Customer;
using Domain.ViewModels.Supplier;
using Domain.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public interface IAuthService
    {
        Task<JwtResponseViewModel> Login(LoginViewModel model);
        Task<RegisterResponseViewModel> Register(RegisterViewModel model);
        Task<CustomerRegisterResponseViewModel> RegisterCustomer(CustomerRegisterViewModel model);
        Task<SupplierRegisterResponseViewModel> RegisterSupplier(SupplierRegisterViewModel model);
        Task<UserProfileViewModel> GetUserProfile(Guid userId);
        Task<bool> ChangePassword(Guid userId, ChangePasswordViewModel model);
        Task EnsureAdminUserExists();
    }
}
