using Application.Services.AdminService;
using Domain.Models;
using Domain.ViewModels.Admin;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.User;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<UserType> _userTypeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SalesOrder> _salesOrderRepository;
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public AdminService(
            IRepository<UserType> userTypeRepository,
            IRepository<User> userRepository,
            IRepository<Customer> customerRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Item> itemRepository,
            IRepository<Category> categoryRepository,
            IRepository<SalesOrder> salesOrderRepository,
            IRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _userTypeRepository = userTypeRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _salesOrderRepository = salesOrderRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<IEnumerable<UserTypeViewModel>> GetAllUserTypes()
        {
            IEnumerable<UserType> userTypes = await _userTypeRepository.GetAll();

            return userTypes.Select(ut => new UserTypeViewModel
            {
                Id = ut.Id,
                Name = ut.Name
            });
        }

        public async Task<UserTypeViewModel> GetUserTypeById(Guid id)
        {
            UserType userType = await _userTypeRepository.GetById(id);

            if (userType == null)
            {
                throw new InvalidOperationException($"UserType with ID {id} not found.");
            }

            return new UserTypeViewModel
            {
                Id = userType.Id,
                Name = userType.Name
            };
        }

        public async Task<UserTypeViewModel> CreateUserType(UserTypeCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var existingUserType = await _userTypeRepository.FindSingle(ut => ut.Name == model.Name);
            if (existingUserType != null)
            {
                throw new InvalidOperationException($"UserType with name {model.Name} already exists.");
            }

            UserType userType = new UserType
            {
                Name = model.Name,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            UserType createdUserType = await _userTypeRepository.Add(userType);

            return new UserTypeViewModel
            {
                Id = createdUserType.Id,
                Name = createdUserType.Name
            };
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
                throw new InvalidOperationException($"UserType with ID {model.Id} not found.");
            }

            var existingUserType = await _userTypeRepository.FindSingle(ut => ut.Name == model.Name && ut.Id != model.Id);
            if (existingUserType != null)
            {
                throw new InvalidOperationException($"UserType with name {model.Name} already exists.");
            }

            userType.Name = model.Name;
            userType.ModifiedDate = DateTime.UtcNow;

            await _userTypeRepository.Update(userType);

            return new UserTypeViewModel
            {
                Id = userType.Id,
                Name = userType.Name
            };
        }

        public async Task<bool> DeleteUserType(Guid id)
        {
            UserType userType = await _userTypeRepository.GetById(id);

            if (userType == null)
            {
                return false;
            }

            var users = await _userRepository.FindAll(u => u.UserTypeId == id);
            if (users.Any())
            {
                throw new InvalidOperationException("Cannot delete UserType because it is in use by one or more users.");
            }

            await _userTypeRepository.Delete(userType);
            return true;
        }

        public async Task<IEnumerable<UserProfileViewModel>> GetAllUsers()
        {
            IEnumerable<User> users = await _userRepository.GetAll();

            List<UserProfileViewModel> userViewModels = new List<UserProfileViewModel>();

            foreach (var user in users)
            {
                var userType = await _userTypeRepository.GetById(user.UserTypeId);

                userViewModels.Add(new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    UserType = userType?.Name ?? "Unknown"
                });
            }

            return userViewModels;
        }

        public async Task<UserProfileViewModel> GetUserById(Guid id)
        {
            User user = await _userRepository.GetById(id);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            var userType = await _userTypeRepository.GetById(user.UserTypeId);

            return new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                UserType = userType?.Name ?? "Unknown"
            };
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            User user = await _userRepository.GetById(id);

            if (user == null)
            {
                return false;
            }

            var userType = await _userTypeRepository.GetById(user.UserTypeId);
            if (userType?.Name == "Admin" && user.Username == "admin")
            {
                throw new InvalidOperationException("Cannot delete the system administrator account.");
            }

            var customer = await _customerRepository.FindSingle(c => c.UserId == id);
            if (customer != null)
            {
                await _customerRepository.Delete(customer);
            }

            var supplier = await _supplierRepository.FindSingle(s => s.UserId == id);
            if (supplier != null)
            {
                await _supplierRepository.Delete(supplier);
            }

            await _userRepository.Delete(user);
            return true;
        }

        public async Task<DashboardViewModel> GetDashboardStats()
        {
            int customersCount = (await _customerRepository.GetAll()).Count();
            int suppliersCount = (await _supplierRepository.GetAll()).Count();
            int categoriesCount = (await _categoryRepository.GetAll()).Count();
            int itemsCount = (await _itemRepository.GetAll()).Count();

            // Get sales orders
            var salesOrders = await _salesOrderRepository.GetAll();
            decimal salesTotal = salesOrders.Sum(so => so.TotalAmount);
            int salesCount = salesOrders.Count();

            // Get purchase orders
            var purchaseOrders = await _purchaseOrderRepository.GetAll();
            decimal purchaseTotal = purchaseOrders.Sum(po => po.TotalAmount);
            int purchaseCount = purchaseOrders.Count();

            // Get recent sales
            var recentSales = salesOrders
                .OrderByDescending(so => so.OrderDate)
                .Take(5)
                .Select(so => new RecentOrderViewModel
                {
                    Id = so.Id,
                    OrderNo = so.OrderNo,
                    OrderDate = so.OrderDate,
                    TotalAmount = so.TotalAmount
                });

            // Get recent purchases
            var recentPurchases = purchaseOrders
                .OrderByDescending(po => po.OrderDate)
                .Take(5)
                .Select(po => new RecentOrderViewModel
                {
                    Id = po.Id,
                    OrderNo = po.OrderNo,
                    OrderDate = po.OrderDate,
                    TotalAmount = po.TotalAmount
                });

            return new DashboardViewModel
            {
                CustomersCount = customersCount,
                SuppliersCount = suppliersCount,
                CategoriesCount = categoriesCount,
                ItemsCount = itemsCount,
                SalesTotal = salesTotal,
                SalesCount = salesCount,
                PurchaseTotal = purchaseTotal,
                PurchaseCount = purchaseCount,
                RecentSales = recentSales.ToList(),
                RecentPurchases = recentPurchases.ToList()
            };
        }
    }
}