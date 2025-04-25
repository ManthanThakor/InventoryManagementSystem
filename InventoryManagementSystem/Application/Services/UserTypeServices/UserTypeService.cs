//using Domain.Models;
//using Domain.ViewModels.User;
//using Infrastructure.Repository;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Services.UserTypeServices
//{
//    public class UserTypeService : IUserTypeService
//    {
//        private readonly IRepository<UserType> _userTypeRepository;

//        public UserTypeService(IRepository<UserType> userTypeRepository)
//        {
//            _userTypeRepository = userTypeRepository;
//        }

//        public async Task<IEnumerable<UserTypeViewModel>> GetAllUserTypes()
//        {
//            var userTypes = await _userTypeRepository.GetAll();

//            return userTypes.Select(ut => new UserTypeViewModel
//            {
//                Id = ut.Id,
//                Name = ut.Name
//            });
//        }

//        public async Task<UserTypeViewModel> GetUserTypeById(Guid id)
//        {
//            var userType = await _userTypeRepository.GetById(id);
//            if (userType == null)
//            {
//                return null;
//            }

//            return new UserTypeViewModel
//            {
//                Id = userType.Id,
//                Name = userType.Name
//            };
//        }

//        public async Task<UserTypeViewModel> CreateUserType(UserTypeCreateViewModel model)
//        {
//            var existingUserType = await _userTypeRepository.FindSingle(ut => ut.Name == model.Name);
//            if (existingUserType != null)
//            {
//                return null;
//            }

//            var userType = new UserType
//            {
//                Id = Guid.NewGuid(),
//                Name = model.Name,
//                CreatedDate = DateTime.UtcNow,
//                ModifiedDate = DateTime.UtcNow
//            };

//            await _userTypeRepository.Add(userType);
//            await _userTypeRepository.SaveChangesMethod();

//            return new UserTypeViewModel
//            {
//                Id = userType.Id,
//                Name = userType.Name
//            };
//        }

//        public async Task<UserTypeViewModel> UpdateUserType(UserTypeUpdateViewModel model)
//        {
//            var userType = await _userTypeRepository.GetById(model.Id);
//            if (userType == null)
//            {
//                return null;
//            }

//            var existingUserType = await _userTypeRepository.FindSingle(ut => ut.Name == model.Name && ut.Id != model.Id);
//            if (existingUserType != null)
//            {
//                return null;
//            }

//            userType.Name = model.Name;
//            userType.ModifiedDate = DateTime.UtcNow;

//            await _userTypeRepository.Update(userType);
//            await _userTypeRepository.SaveChangesMethod();

//            return new UserTypeViewModel
//            {
//                Id = userType.Id,
//                Name = userType.Name
//            };
//        }

//        public async Task<bool> DeleteUserType(Guid id)
//        {
//            var userType = await _userTypeRepository.GetById(id);
//            if (userType == null)
//            {
//                return false;
//            }

//            await _userTypeRepository.Delete(userType);
//            await _userTypeRepository.SaveChangesMethod();

//            return true;
//        }
//    }
//}
