using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Admin;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.User;

namespace Application.Services.AdminService
{
    public interface IAdminService
    {
        Task<IEnumerable<UserTypeViewModel>> GetAllUserTypes();
        Task<UserTypeViewModel> GetUserTypeById(Guid id);
        Task<UserTypeViewModel> CreateUserType(UserTypeCreateViewModel model);
        Task<UserTypeViewModel> UpdateUserType(UserTypeUpdateViewModel model);
        Task<bool> DeleteUserType(Guid id);
        Task<IEnumerable<UserProfileViewModel>> GetAllUsers();
        Task<UserProfileViewModel> GetUserById(Guid id);
        Task<bool> DeleteUser(Guid id);
        Task<DashboardViewModel> GetDashboardStats();
    }
}
