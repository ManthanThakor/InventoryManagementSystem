using Application.Services.JwtServices;
using Application.Services.PasswordServices;
using Domain.Models;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.Customer;
using Domain.ViewModels.JwtUser;
using Domain.ViewModels.Supplier;
using Domain.ViewModels.User;
using Infrastructure.Repository;

namespace Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserType> _userTypeRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(
            IRepository<User> userRepository,
            IRepository<UserType> userTypeRepository,
            IRepository<Customer> customerRepository,
            IRepository<Supplier> supplierRepository,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<JwtResponseViewModel> Login(LoginViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            User user = await _userRepository.FindSingle(u => u.Username == model.Username);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!_passwordService.VerifyPassword(model.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid password.");
            }

            UserType userType = await _userTypeRepository.GetById(user.UserTypeId);

            JwtUserViewModel jwtUser = new JwtUserViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                UserType = userType.Name
            };

            (string token, DateTime expiration) = _jwtService.GenerateToken(jwtUser);

            JwtResponseViewModel response = new JwtResponseViewModel
            {
                Token = token,
                Expiration = expiration,
                Username = user.Username,
                UserType = userType.Name,
                UserId = user.Id
            };
            return response;

        }

        public async Task<RegisterResponseViewModel> Register(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            User existingUser = await _userRepository.FindSingle(u => u.Username == model.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            UserType userType = await _userTypeRepository.GetById(model.UserTypeId);
            if (userType == null)
            {
                throw new InvalidOperationException("Invalid user type.");
            }

            User newUser = new User
            {
                FullName = model.FullName,
                Username = model.Username,
                PasswordHash = _passwordService.HashPassword(model.Password),
                UserTypeId = model.UserTypeId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            User registeredUser = await _userRepository.Add(newUser);

            return new RegisterResponseViewModel
            {
                Succeeded = true,
                Message = "Registration successful.",
                UserId = registeredUser.Id,
                UserTypeId = registeredUser.UserTypeId,
                UserTypeName = userType.Name
            };
        }

        public async Task<CustomerRegisterResponseViewModel> RegisterCustomer(CustomerRegisterViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            RegisterViewModel registerModel = new RegisterViewModel
            {
                FullName = model.FullName,
                Username = model.Username,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                UserTypeId = model.UserTypeId
            };

            RegisterResponseViewModel registerResponse = await Register(registerModel);

            Customer customer = new Customer
            {
                Name = model.Name,
                Address = model.Address,
                Contact = model.Contact,
                UserId = registerResponse.UserId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Customer registeredCustomer = await _customerRepository.Add(customer);

            return new CustomerRegisterResponseViewModel
            {
                Succeeded = registerResponse.Succeeded,
                Message = registerResponse.Message,
                UserId = registerResponse.UserId,
                UserTypeId = registerResponse.UserTypeId,
                UserTypeName = registerResponse.UserTypeName,
                CustomerId = registeredCustomer.Id
            };
        }

        public async Task<SupplierRegisterResponseViewModel> RegisterSupplier(SupplierRegisterViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            RegisterViewModel registerModel = new RegisterViewModel
            {
                FullName = model.FullName,
                Username = model.Username,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                UserTypeId = model.UserTypeId
            };

            RegisterResponseViewModel registerResponse = await Register(registerModel);

            Supplier supplier = new Supplier
            {
                Name = model.Name,
                Address = model.Address,
                Contact = model.Contact,
                UserId = registerResponse.UserId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Supplier registeredSupplier = await _supplierRepository.Add(supplier);

            return new SupplierRegisterResponseViewModel
            {
                Succeeded = registerResponse.Succeeded,
                Message = registerResponse.Message,
                UserId = registerResponse.UserId,
                UserTypeId = registerResponse.UserTypeId,
                UserTypeName = registerResponse.UserTypeName,
                SupplierId = registeredSupplier.Id
            };
        }

        public async Task<UserProfileViewModel> GetUserProfile(Guid userId)
        {
            User user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            UserType userType = await _userTypeRepository.GetById(user.UserTypeId);

            return new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                UserType = userType.Name
            };
        }

        public async Task<bool> ChangePassword(Guid userId, ChangePasswordViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            User user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!_passwordService.VerifyPassword(model.CurrentPassword, user.PasswordHash))
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            user.PasswordHash = _passwordService.HashPassword(model.NewPassword);
            user.ModifiedDate = DateTime.UtcNow;

            await _userRepository.Update(user);
            return true;
        }


    }
}