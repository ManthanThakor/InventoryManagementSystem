using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.Customer;
using Domain.ViewModels.Orders;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<CustomerItem> _customerItemRepository;
        private readonly IRepository<Item> _itemRepository;

        public CustomerService(
            IRepository<Customer> customerRepository,
            IRepository<User> userRepository,
            IRepository<CustomerItem> customerItemRepository,
            IRepository<Item> itemRepository)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _customerItemRepository = customerItemRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomers()
        {
            IEnumerable<Customer> customers = await _customerRepository.GetAll();

            List<CustomerViewModel> customerViewModels = new List<CustomerViewModel>();

            foreach (var customer in customers)
            {
                var user = await _userRepository.GetById(customer.UserId);

                if (user != null)
                {
                    CustomerViewModel customerViewModel = new CustomerViewModel
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Address = customer.Address,
                        Contact = customer.Contact,
                        User = new UserProfileViewModel
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Username = user.Username,
                            UserType = "Customer" // Assuming UserType is already loaded
                        }
                    };

                    customerViewModels.Add(customerViewModel);
                }
            }

            return customerViewModels;
        }

        public async Task<CustomerDetailViewModel> GetCustomerById(Guid id)
        {
            Customer customer = await _customerRepository.GetById(id);

            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {id} not found.");
            }

            var user = await _userRepository.GetById(customer.UserId);

            if (user == null)
            {
                throw new InvalidOperationException($"User associated with customer ID {id} not found.");
            }

            // Get customer items
            var customerItems = await _customerItemRepository.FindAll(ci => ci.CustomerId == id);
            var customerItemViewModels = new List<CustomerItemViewModel>();

            foreach (var customerItem in customerItems)
            {
                var item = await _itemRepository.GetById(customerItem.ItemId);

                if (item != null)
                {
                    var itemViewModel = new Domain.ViewModels.Item.ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        GSTPercent = item.GSTPercent,
                        PurchasePrice = item.PurchasePrice,
                        SellingPrice = item.SellingPrice
                    };

                    var customerItemViewModel = new CustomerItemViewModel
                    {
                        Id = customerItem.Id,
                        Item = itemViewModel,
                        GSTAmount = customerItem.GSTAmount,
                        TotalAmount = customerItem.TotalAmount
                    };

                    customerItemViewModels.Add(customerItemViewModel);
                }
            }

            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Contact = customer.Contact,
                User = new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    UserType = "Customer" // Assuming UserType is already loaded
                },
                CustomerItems = customerItemViewModels,
                // We'll fetch sales orders in a separate method
                SalesOrders = new List<Domain.ViewModels.SalesOrder.SalesOrderViewModel>()
            };

            return customerDetailViewModel;
        }

        public async Task<CustomerViewModel> UpdateCustomer(CustomerUpdateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Customer customer = await _customerRepository.GetById(model.Id);

            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {model.Id} not found.");
            }

            customer.Name = model.Name;
            customer.Address = model.Address;
            customer.Contact = model.Contact;
            customer.ModifiedDate = DateTime.UtcNow;

            await _customerRepository.Update(customer);

            var user = await _userRepository.GetById(customer.UserId);

            CustomerViewModel customerViewModel = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Contact = customer.Contact,
                User = user != null ? new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    UserType = "Customer" // Assuming UserType is already loaded
                } : null
            };

            return customerViewModel;
        }

        public async Task<bool> DeleteCustomer(Guid id)
        {
            Customer customer = await _customerRepository.GetById(id);

            if (customer == null)
            {
                return false;
            }

            // First delete related customer items
            var customerItems = await _customerItemRepository.FindAll(ci => ci.CustomerId == id);

            foreach (var customerItem in customerItems)
            {
                await _customerItemRepository.Delete(customerItem);
            }

            // Then delete the customer
            await _customerRepository.Delete(customer);

            return true;
        }

        public async Task<IEnumerable<CustomerItemViewModel>> GetItemsByCustomer(Guid customerId)
        {
            Customer customer = await _customerRepository.GetById(customerId);

            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {customerId} not found.");
            }

            var customerItems = await _customerItemRepository.FindAll(ci => ci.CustomerId == customerId);
            var customerItemViewModels = new List<CustomerItemViewModel>();

            foreach (var customerItem in customerItems)
            {
                var item = await _itemRepository.GetById(customerItem.ItemId);

                if (item != null)
                {
                    var itemViewModel = new Domain.ViewModels.Item.ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        GSTPercent = item.GSTPercent,
                        PurchasePrice = item.PurchasePrice,
                        SellingPrice = item.SellingPrice
                    };

                    var customerItemViewModel = new CustomerItemViewModel
                    {
                        Id = customerItem.Id,
                        Item = itemViewModel,
                        GSTAmount = customerItem.GSTAmount,
                        TotalAmount = customerItem.TotalAmount
                    };

                    customerItemViewModels.Add(customerItemViewModel);
                }
            }

            return customerItemViewModels;
        }

        public async Task<CustomerItemViewModel> AddCustomerItem(CustomerItemCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Verify customer exists
            var customer = await _customerRepository.GetById(model.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {model.CustomerId} not found.");
            }

            // Verify item exists
            var item = await _itemRepository.GetById(model.ItemId);
            if (item == null)
            {
                throw new InvalidOperationException($"Item with ID {model.ItemId} not found.");
            }

            CustomerItem customerItem = new CustomerItem
            {
                ItemId = model.ItemId,
                CustomerId = model.CustomerId,
                GSTAmount = model.GSTAmount,
                TotalAmount = model.TotalAmount,
                SalesOrderId = model.SalesOrderId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            CustomerItem createdCustomerItem = await _customerItemRepository.Add(customerItem);

            var itemViewModel = new Domain.ViewModels.Item.ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                GSTPercent = item.GSTPercent,
                PurchasePrice = item.PurchasePrice,
                SellingPrice = item.SellingPrice
            };

            CustomerItemViewModel customerItemViewModel = new CustomerItemViewModel
            {
                Id = createdCustomerItem.Id,
                Item = itemViewModel,
                GSTAmount = createdCustomerItem.GSTAmount,
                TotalAmount = createdCustomerItem.TotalAmount
            };

            return customerItemViewModel;
        }

        public async Task<bool> RemoveCustomerItem(Guid customerItemId)
        {
            CustomerItem customerItem = await _customerItemRepository.GetById(customerItemId);

            if (customerItem == null)
            {
                return false;
            }

            await _customerItemRepository.Delete(customerItem);
            return true;
        }

        public async Task<IEnumerable<CustomerViewModel>> SearchCustomers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<CustomerViewModel>();
            }

            string lowerSearchTerm = searchTerm.ToLower();

            IEnumerable<Customer> customers = await _customerRepository.FindAll(c =>
                c.Name.ToLower().Contains(lowerSearchTerm) ||
                c.Address.ToLower().Contains(lowerSearchTerm) ||
                c.Contact.ToLower().Contains(lowerSearchTerm));

            List<CustomerViewModel> customerViewModels = new List<CustomerViewModel>();

            foreach (var customer in customers)
            {
                var user = await _userRepository.GetById(customer.UserId);

                if (user != null)
                {
                    CustomerViewModel customerViewModel = new CustomerViewModel
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Address = customer.Address,
                        Contact = customer.Contact,
                        User = new UserProfileViewModel
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Username = user.Username,
                            UserType = "Customer" // Assuming UserType is already loaded
                        }
                    };

                    customerViewModels.Add(customerViewModel);
                }
            }

            return customerViewModels;
        }
    }
}