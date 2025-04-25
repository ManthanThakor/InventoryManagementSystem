using Domain.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserTypeServices
{
    public interface IUserTypeService
    {
        Task<IEnumerable<UserTypeViewModel>> GetAllUserTypes();
        Task<UserTypeViewModel> GetUserTypeById(Guid id);
        Task<UserTypeViewModel> CreateUserType(UserTypeCreateViewModel model);
        Task<UserTypeViewModel> UpdateUserType(UserTypeUpdateViewModel model);
        Task<bool> DeleteUserType(Guid id);
    }
}
