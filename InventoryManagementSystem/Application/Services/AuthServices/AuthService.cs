//using Application.Services.JwtServices;
//using Application.Services.PasswordServices;
//using Domain.Models;
//using Domain.ViewModels.Authentication;
//using Domain.ViewModels.Customer;
//using Domain.ViewModels.Supplier;
//using Domain.ViewModels.User;
//using Infrastructure.Repository;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Services.AuthServices
//{
//    public class AuthService : IAuthService
//    {
//        private readonly IRepository<User> _userRepository;
//        private readonly IRepository<UserType> _userTypeRepository;
//        private readonly IRepository<Customer> _customerRepository;
//        private readonly IRepository<Supplier> _supplierRepository;
//        private readonly IPasswordService _passwordService;
//        private readonly IJwtService _jwtService;

//        public AuthService(
//            IRepository<User> userRepository,
//            IRepository<UserType> userTypeRepository,
//            IRepository<Customer> customerRepository,
//            IRepository<Supplier> supplierRepository,
//            IPasswordService passwordService,
//            IJwtService jwtService)
//        {
//            _userRepository = userRepository;
//            _userTypeRepository = userTypeRepository;
//            _customerRepository = customerRepository;
//            _supplierRepository = supplierRepository;
//            _passwordService = passwordService;
//            _jwtService = jwtService;
//        }

//        public async Task<JwtResponseViewModel> Login(LoginViewModel model)
//        {
//            var user = await _userRepository.FindSingle(u => u.Username == model.Username);
//            if (user == null)
//            {
//                return null;
//            }

//            if (!_passwordService.VerifyPassword(model.Password, user.PasswordHash))
//            {
//                return null;
//            }

//            var userType = await _userTypeRepository.GetById(user.UserTypeId);
//            var token = _jwtService.GenerateToken(user.Id, user.Username, userType.Name);

//            return new JwtResponseViewModel
//            {
//                Token = token.token,
//                Expiration = token.expiration,
//                Username = user.Username,
//                UserType = userType.Name,
//                UserId = user.Id
//            };
//        }

//        public async Task<RegisterResponseViewModel> Register(RegisterViewModel model)
//        {
//            if (model.Password != model.ConfirmPassword)
//            {
//                return new RegisterResponseViewModel
//                {
//                    Succeeded = false,
//                    Message = "Password and confirmation password do not match."
//                };
//            }

//            var existingUser = await _userRepository.FindSingle(u => u.Username == model.Username);
//            if (existingUser != null)
//            {
//                return new RegisterResponseViewModel
//                {
//                    Succeeded = false,
//                    Message = "Username already exists."
//                };
//            }

//            var userType = await _userTypeRepository.GetById(model.UserTypeId);
//            if (userType == null)
//            {
//                return new RegisterResponseViewModel
//                {
//                    Succeeded = false,
//                    Message = "Invalid user type."
//                };
//            }

//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                FullName = model.FullName,
//                Username = model.Username,
//                PasswordHash = _passwordService.HashPassword(model.Password),
//                UserTypeId = model.UserTypeId,
//                CreatedDate = DateTime.UtcNow,
//                ModifiedDate = DateTime.UtcNow
//            };

//            await _userRepository.Add(user);
//            await _userRepository.SaveChangesMethod();

//            return new RegisterResponseViewModel
//            {
//                Succeeded = true,
//                Message = "User registered successfully.",
//                UserId = user.Id,
//                UserTypeId = user.UserTypeId,
//                UserTypeName = userType.Name
//            };
//        }

//        public async Task<CustomerRegisterResponseViewModel> RegisterCustomer(CustomerRegisterViewModel model)
//        {
//            var registerResult = await Register(model);
//            if (!registerResult.Succeeded)
//            {
//                return new CustomerRegisterResponseViewModel
//                {
//                    Succeeded = false,
//                    Message = registerResult.Message
//                };
//            }

//            var customer = new Customer
//            {
//                Id = Guid.NewGuid(),
//                Name = model.Name,
//                Address = model.Address,
//                Contact = model.Contact,
//                UserId = registerResult.UserId,
//                CreatedDate = DateTime.UtcNow,
//                ModifiedDate = DateTime.UtcNow
//            };

//            await _customerRepository.Add(customer);
//            await _customerRepository.SaveChangesMethod();

//            return new CustomerRegisterResponseViewModel
//            {
//                Succeeded = true,
//                Message = "Customer registered successfully.",
//                UserId = registerResult.UserId,
//                UserTypeId = registerResult.UserTypeId,
//                UserTypeName = registerResult.UserTypeName,
//                CustomerId = customer.Id
//            };
//        }

//        public async Task<SupplierRegisterResponseViewModel> RegisterSupplier(SupplierRegisterViewModel model)
//        {
//            var registerResult = await Register(model);
//            if (!registerResult.Succeeded)
//            {
//                return new SupplierRegisterResponseViewModel
//                {
//                    Succeeded = false,
//                    Message = registerResult.Message
//                };
//            }

//            var supplier = new Supplier
//            {
//                Id = Guid.NewGuid(),
//                Name = model.Name,
//                Address = model.Address,
//                Contact = model.Contact,
//                UserId = registerResult.UserId,
//                CreatedDate = DateTime.UtcNow,
//                ModifiedDate = DateTime.UtcNow
//            };

//            await _supplierRepository.Add(supplier);
//            await _supplierRepository.SaveChangesMethod();

//            return new SupplierRegisterResponseViewModel
//            {
//                Succeeded = true,
//                Message = "Supplier registered successfully.",
//                UserId = registerResult.UserId,
//                UserTypeId = registerResult.UserTypeId,
//                UserTypeName = registerResult.UserTypeName,
//                SupplierId = supplier.Id
//            };
//        }

//        public async Task<UserProfileViewModel> GetUserProfile(Guid userId)
//        {
//            var user = await _userRepository.GetById(userId);
//            if (user == null)
//            {
//                return null;
//            }

//            var userType = await _userTypeRepository.GetById(user.UserTypeId);

//            return new UserProfileViewModel
//            {
//                Id = user.Id,
//                FullName = user.FullName,
//                Username = user.Username,
//                UserType = userType.Name
//            };
//        }

//        public async Task<bool> ChangePassword(Guid userId, ChangePasswordViewModel model)
//        {
//            var user = await _userRepository.GetById(userId);
//            if (user == null)
//            {
//                return false;
//            }

//            if (!_passwordService.VerifyPassword(model.CurrentPassword, user.PasswordHash))
//            {
//                return false;
//            }

//            if (model.NewPassword != model.ConfirmPassword)
//            {
//                return false;
//            }

//            user.PasswordHash = _passwordService.HashPassword(model.NewPassword);
//            user.ModifiedDate = DateTime.UtcNow;

//            await _userRepository.Update(user);
//            await _userRepository.SaveChangesMethod();

//            return true;
//        }
//    }
//}
