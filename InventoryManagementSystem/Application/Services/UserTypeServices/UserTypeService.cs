using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.User;
using Infrastructure.Repository;

namespace Application.Services.UserTypeServices
{
    public class UserTypeService : IUserTypeService
    {
        private readonly IRepository<UserType> _userTypeRepository;

        public UserTypeService(IRepository<UserType> userTypeRepository)
        {
            _userTypeRepository = userTypeRepository;
        }

        public async Task<IEnumerable<UserTypeViewModel>> GetAllUserTypes()
        {

            IEnumerable<UserType> userTypes = await _userTypeRepository.GetAll();

            List<UserTypeViewModel> userTypeViewModels = new List<UserTypeViewModel>();

            foreach (UserType userType in userTypes)
            {
                if (userType != null)
                {
                    UserTypeViewModel userTypeViewModel = new UserTypeViewModel
                    {
                        Id = userType.Id,
                        Name = userType.Name
                    };

                    userTypeViewModels.Add(userTypeViewModel);
                }
            }
            return userTypeViewModels;
        }

        public async Task<UserTypeViewModel> GetUserTypeById(Guid id)
        {
            UserType userType = await _userTypeRepository.GetById(id);

            if (userType == null)
            {
                throw new InvalidOperationException($"UserType with ID {id} not found.");
            }

            UserTypeViewModel userTypeViewModel = new UserTypeViewModel
            {
                Id = userType.Id,
                Name = userType.Name
            };

            return userTypeViewModel;
        }

        public async Task<UserTypeViewModel> CreateUserType(UserTypeCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            UserType userType = new UserType
            {
                Name = model.Name
            };

            UserType createdUserType = await _userTypeRepository.Add(userType);

            if (createdUserType == null)
            {
                throw new InvalidOperationException("Failed to create UserType.");
            }

            UserTypeViewModel userTypeViewModel = new UserTypeViewModel
            {
                Id = createdUserType.Id,
                Name = createdUserType.Name
            };

            return userTypeViewModel;
        }

        public async Task<UserTypeViewModel> UpdateUserType(UserTypeUpdateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            UserType userType = await _userTypeRepository.GetById(model.Id);

            if (userType == null)
            {
                throw new InvalidOperationException("UserType not found.");
            }

            userType.Name = model.Name;

            await _userTypeRepository.Update(userType);

            UserTypeViewModel updatedUserTypeViewModel = new UserTypeViewModel
            {
                Id = userType.Id,
                Name = userType.Name
            };

            return updatedUserTypeViewModel;
        }

        public async Task<bool> DeleteUserType(Guid id)
        {
            UserType userType = await _userTypeRepository.GetById(id);

            if (userType == null)
            {
                return false;
            }

            await _userTypeRepository.Delete(userType);

            return true;
        }
    }
}
