using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Item;
using Domain.ViewModels.Orders;
using Domain.ViewModels.Supplier;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.PurchaseOrderServices
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<SupplierItem> _supplierItemRepository;
        private readonly IRepository<Item> _itemRepository;

        public PurchaseOrderService(
            IRepository<PurchaseOrder> purchaseOrderRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<SupplierItem> supplierItemRepository,
            IRepository<Item> itemRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _supplierRepository = supplierRepository;
            _supplierItemRepository = supplierItemRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<PurchaseOrderListViewModel>> GetAllPurchaseOrders()
        {
            IEnumerable<PurchaseOrder> purchaseOrders = await _purchaseOrderRepository.GetAll();

            List<PurchaseOrderListViewModel> purchaseOrderViewModels = new List<PurchaseOrderListViewModel>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                var supplier = await _supplierRepository.GetById(purchaseOrder.SupplierId);

                if (supplier != null)
                {
                    PurchaseOrderListViewModel purchaseOrderViewModel = new PurchaseOrderListViewModel
                    {
                        Id = purchaseOrder.Id,
                        OrderNo = purchaseOrder.OrderNo,
                        OrderDate = purchaseOrder.OrderDate,
                        TotalAmount = purchaseOrder.TotalAmount,
                        SupplierName = supplier.Name
                    };

                    purchaseOrderViewModels.Add(purchaseOrderViewModel);
                }
            }

            return purchaseOrderViewModels;
        }

        public async Task<PurchaseOrderDetailViewModel> GetPurchaseOrderById(Guid id)
        {
            PurchaseOrder purchaseOrder = await _purchaseOrderRepository.GetById(id);

            if (purchaseOrder == null)
            {
                throw new InvalidOperationException($"Purchase Order with ID {id} not found.");
            }

            Supplier supplier = await _supplierRepository.GetById(purchaseOrder.SupplierId);

            if (supplier == null)
            {
                throw new InvalidOperationException($"Supplier associated with Purchase Order ID {id} not found.");
            }

            IEnumerable<SupplierItem> supplierItems = await _supplierItemRepository.FindAll(si => si.PurchaseOrderId == id);
            List<SupplierItemViewModel> supplierItemViewModels = new List<SupplierItemViewModel>();

            foreach (var supplierItem in supplierItems)
            {
                var item = await _itemRepository.GetById(supplierItem.ItemId);

                if (item != null)
                {
                    ItemViewModel itemViewModel = new ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        GSTPercent = item.GSTPercent,
                        PurchasePrice = item.PurchasePrice,
                        SellingPrice = item.SellingPrice
                    };

                    SupplierItemViewModel supplierItemViewModel = new SupplierItemViewModel
                    {
                        Id = supplierItem.Id,
                        Item = itemViewModel,
                        GSTAmount = supplierItem.GSTAmount,
                        TotalAmount = supplierItem.TotalAmount
                    };

                    supplierItemViewModels.Add(supplierItemViewModel);
                }
            }

            SupplierViewModel supplierViewModel = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Address = supplier.Address,
                Contact = supplier.Contact
            };

            PurchaseOrderDetailViewModel purchaseOrderDetailViewModel = new PurchaseOrderDetailViewModel
            {
                Id = purchaseOrder.Id,
                OrderNo = purchaseOrder.OrderNo,
                OrderDate = purchaseOrder.OrderDate,
                TotalAmount = purchaseOrder.TotalAmount,
                Supplier = supplierViewModel,
                SupplierItems = supplierItemViewModels
            };

            return purchaseOrderDetailViewModel;
        }

        public async Task<PurchaseOrderViewModel> CreatePurchaseOrder(PurchaseOrderCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Supplier supplier = await _supplierRepository.GetById(model.SupplierId);
            if (supplier == null)
            {
                throw new InvalidOperationException($"Supplier with ID {model.SupplierId} not found.");
            }

            string orderNo = $"PO-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";

            PurchaseOrder purchaseOrder = new PurchaseOrder
            {
                OrderNo = orderNo,
                SupplierId = model.SupplierId,
                OrderDate = model.OrderDate,
                TotalAmount = 0,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            PurchaseOrder createdPurchaseOrder = await _purchaseOrderRepository.Add(purchaseOrder);

            decimal totalAmount = 0;

            if (model.Items != null && model.Items.Any())
            {
                foreach (var itemModel in model.Items)
                {
                    var item = await _itemRepository.GetById(itemModel.ItemId);
                    if (item == null)
                    {
                        throw new InvalidOperationException($"Item with ID {itemModel.ItemId} not found.");
                    }

                    decimal gstAmount = (item.PurchasePrice * item.GSTPercent) / 100;
                    decimal totalItemAmount = item.PurchasePrice + gstAmount;

                    SupplierItem supplierItem = new SupplierItem
                    {
                        ItemId = itemModel.ItemId,
                        SupplierId = model.SupplierId,
                        PurchaseOrderId = createdPurchaseOrder.Id,
                        GSTAmount = gstAmount,
                        TotalAmount = totalItemAmount,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };

                    await _supplierItemRepository.Add(supplierItem);

                    totalAmount += totalItemAmount;
                }
            }

            createdPurchaseOrder.TotalAmount = totalAmount;
            await _purchaseOrderRepository.Update(createdPurchaseOrder);

            SupplierViewModel supplierViewModel = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Address = supplier.Address,
                Contact = supplier.Contact
            };

            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel
            {
                Id = createdPurchaseOrder.Id,
                OrderNo = createdPurchaseOrder.OrderNo,
                OrderDate = createdPurchaseOrder.OrderDate,
                TotalAmount = createdPurchaseOrder.TotalAmount,
                Supplier = supplierViewModel
            };

            return purchaseOrderViewModel;
        }

        public async Task<bool> DeletePurchaseOrder(Guid id)
        {
            PurchaseOrder purchaseOrder = await _purchaseOrderRepository.GetById(id);

            if (purchaseOrder == null)
            {
                return false;
            }

            IEnumerable<SupplierItem> supplierItems = await _supplierItemRepository.FindAll(si => si.PurchaseOrderId == id);

            foreach (var supplierItem in supplierItems)
            {
                await _supplierItemRepository.Delete(supplierItem);
            }

            await _purchaseOrderRepository.Delete(purchaseOrder);

            return true;
        }

        public async Task<IEnumerable<PurchaseOrderListViewModel>> GetPurchaseOrdersBySupplier(Guid supplierId)
        {
            Supplier supplier = await _supplierRepository.GetById(supplierId);

            if (supplier == null)
            {
                throw new InvalidOperationException($"Supplier with ID {supplierId} not found.");
            }

            var purchaseOrders = await _purchaseOrderRepository.FindAll(po => po.SupplierId == supplierId);

            List<PurchaseOrderListViewModel> purchaseOrderViewModels = new List<PurchaseOrderListViewModel>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                PurchaseOrderListViewModel purchaseOrderViewModel = new PurchaseOrderListViewModel
                {
                    Id = purchaseOrder.Id,
                    OrderNo = purchaseOrder.OrderNo,
                    OrderDate = purchaseOrder.OrderDate,
                    TotalAmount = purchaseOrder.TotalAmount,
                    SupplierName = supplier.Name
                };

                purchaseOrderViewModels.Add(purchaseOrderViewModel);
            }

            return purchaseOrderViewModels;
        }

        public async Task<IEnumerable<PurchaseOrderListViewModel>> SearchPurchaseOrders(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<PurchaseOrderListViewModel>();
            }

            string lowerSearchTerm = searchTerm.ToLower();

            IEnumerable<PurchaseOrder> purchaseOrders = await _purchaseOrderRepository.FindAll(po =>
                po.OrderNo.ToLower().Contains(lowerSearchTerm));

            List<PurchaseOrderListViewModel> purchaseOrderViewModels = new List<PurchaseOrderListViewModel>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                var supplier = await _supplierRepository.GetById(purchaseOrder.SupplierId);

                if (supplier != null)
                {
                    PurchaseOrderListViewModel purchaseOrderViewModel = new PurchaseOrderListViewModel
                    {
                        Id = purchaseOrder.Id,
                        OrderNo = purchaseOrder.OrderNo,
                        OrderDate = purchaseOrder.OrderDate,
                        TotalAmount = purchaseOrder.TotalAmount,
                        SupplierName = supplier.Name
                    };

                    purchaseOrderViewModels.Add(purchaseOrderViewModel);
                }
            }

            return purchaseOrderViewModels;
        }
    }
}