using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.Orders;
using Domain.ViewModels.Supplier;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.SupplierServices
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<SupplierItem> _supplierItemRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public SupplierService(
            IRepository<Supplier> supplierRepository,
            IRepository<User> userRepository,
            IRepository<SupplierItem> supplierItemRepository,
            IRepository<Item> itemRepository,
            IRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _supplierRepository = supplierRepository;
            _userRepository = userRepository;
            _supplierItemRepository = supplierItemRepository;
            _itemRepository = itemRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<IEnumerable<SupplierViewModel>> GetAllSuppliers()
        {
            IEnumerable<Supplier> suppliers = await _supplierRepository.GetAll();
            List<SupplierViewModel> supplierViewModels = new List<SupplierViewModel>();

            foreach (var supplier in suppliers)
            {
                var user = await _userRepository.GetById(supplier.UserId);

                if (user != null)
                {
                    SupplierViewModel supplierViewModel = new SupplierViewModel
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        Address = supplier.Address,
                        Contact = supplier.Contact,
                        User = new UserProfileViewModel
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Username = user.Username,
                            UserType = "Supplier"
                        }
                    };
                    supplierViewModels.Add(supplierViewModel);
                }
            }

            return supplierViewModels;
        }

        public async Task<SupplierDetailViewModel> GetSupplierById(Guid id)
        {
            Supplier supplier = await _supplierRepository.GetById(id);
            if (supplier == null)
            {
                throw new InvalidOperationException($"Supplier with ID {id} not found.");
            }

            var user = await _userRepository.GetById(supplier.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"User associated with supplier ID {id} not found.");
            }

            var supplierItems = await _supplierItemRepository.FindAll(si => si.SupplierId == id);
            var supplierItemViewModels = new List<SupplierItemViewModel>();

            foreach (var supplierItem in supplierItems)
            {
                var item = await _itemRepository.GetById(supplierItem.ItemId);
                if (item != null)
                {
                    supplierItemViewModels.Add(new SupplierItemViewModel
                    {
                        Id = supplierItem.Id,
                        Item = new Domain.ViewModels.Item.ItemViewModel
                        {
                            Id = item.Id,
                            Name = item.Name,
                            GSTPercent = item.GSTPercent,
                            PurchasePrice = item.PurchasePrice,
                            SellingPrice = item.SellingPrice
                        },
                        GSTAmount = supplierItem.GSTAmount,
                        TotalAmount = supplierItem.TotalAmount
                    });
                }
            }

            var purchaseOrders = await _purchaseOrderRepository.FindAll(po => po.SupplierId == id);
            var purchaseOrderViewModels = new List<PurchaseOrderListViewModel>();

            foreach (var order in purchaseOrders)
            {
                purchaseOrderViewModels.Add(new PurchaseOrderListViewModel
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount
                });
            }

            return new SupplierDetailViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Address = supplier.Address,
                Contact = supplier.Contact,
                User = new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    UserType = "Supplier"
                },
                SupplierItems = supplierItemViewModels,
                PurchaseOrders = purchaseOrderViewModels
            };
        }

        public async Task<SupplierViewModel> UpdateSupplier(SupplierUpdateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Supplier supplier = await _supplierRepository.GetById(model.Id);
            if (supplier == null)
            {
                throw new InvalidOperationException($"Supplier with ID {model.Id} not found.");
            }

            supplier.Name = model.Name;
            supplier.Address = model.Address;
            supplier.Contact = model.Contact;
            supplier.ModifiedDate = DateTime.UtcNow;
            await _supplierRepository.Update(supplier);

            var user = await _userRepository.GetById(supplier.UserId);
            SupplierViewModel viewModel = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Address = supplier.Address,
                Contact = supplier.Contact
            };

            if (user != null)
            {
                viewModel.User = new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    UserType = "Supplier"
                };
            }

            return viewModel;
        }

        public async Task<bool> DeleteSupplier(Guid id)
        {
            Supplier supplier = await _supplierRepository.GetById(id);
            if (supplier == null)
            {
                return false;
            }

            var supplierItems = await _supplierItemRepository.FindAll(si => si.SupplierId == id);
            foreach (var supplierItem in supplierItems)
            {
                await _supplierItemRepository.Delete(supplierItem);
            }

            await _supplierRepository.Delete(supplier);
            return true;
        }

        public async Task<IEnumerable<SupplierItemViewModel>> GetItemsBySupplier(Guid supplierId)
        {
            Supplier supplier = await _supplierRepository.GetById(supplierId);
            if (supplier == null)
            {
                throw new InvalidOperationException($"Supplier with ID {supplierId} not found.");
            }

            var supplierItems = await _supplierItemRepository.FindAll(si => si.SupplierId == supplierId);
            var supplierItemViewModels = new List<SupplierItemViewModel>();

            foreach (var supplierItem in supplierItems)
            {
                var item = await _itemRepository.GetById(supplierItem.ItemId);
                if (item != null)
                {
                    supplierItemViewModels.Add(new SupplierItemViewModel
                    {
                        Id = supplierItem.Id,
                        Item = new Domain.ViewModels.Item.ItemViewModel
                        {
                            Id = item.Id,
                            Name = item.Name,
                            GSTPercent = item.GSTPercent,
                            PurchasePrice = item.PurchasePrice,
                            SellingPrice = item.SellingPrice
                        },
                        GSTAmount = supplierItem.GSTAmount,
                        TotalAmount = supplierItem.TotalAmount
                    });
                }
            }

            return supplierItemViewModels;
        }

        public async Task<bool> RemoveSupplierItem(Guid supplierItemId)
        {
            SupplierItem supplierItem = await _supplierItemRepository.GetById(supplierItemId);
            if (supplierItem == null)
            {
                return false;
            }

            await _supplierItemRepository.Delete(supplierItem);
            return true;
        }

        public async Task<IEnumerable<SupplierViewModel>> SearchSuppliers(string searchTerm)
        {
            List<SupplierViewModel> supplierViewModels = new List<SupplierViewModel>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                return supplierViewModels;
            }

            string lowerSearchTerm = searchTerm.ToLower();

            IEnumerable<Supplier> suppliers = await _supplierRepository.FindAll(s =>
                s.Name.ToLower().Contains(lowerSearchTerm) ||
                s.Address.ToLower().Contains(lowerSearchTerm) ||
                s.Contact.ToLower().Contains(lowerSearchTerm));

            foreach (var supplier in suppliers)
            {
                var user = await _userRepository.GetById(supplier.UserId);
                SupplierViewModel viewModel = new SupplierViewModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Address = supplier.Address,
                    Contact = supplier.Contact
                };

                if (user != null)
                {
                    viewModel.User = new UserProfileViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Username = user.Username,
                        UserType = "Supplier"
                    };
                }

                supplierViewModels.Add(viewModel);
            }

            return supplierViewModels;
        }
    }
}
