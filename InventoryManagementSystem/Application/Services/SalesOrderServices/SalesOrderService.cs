using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Orders;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.SalesOrderServices
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IRepository<SalesOrder> _salesOrderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerItem> _customerItemRepository;
        private readonly IRepository<Item> _itemRepository;

        public SalesOrderService(
            IRepository<SalesOrder> salesOrderRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerItem> customerItemRepository,
            IRepository<Item> itemRepository)
        {
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _customerItemRepository = customerItemRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<SalesOrderListViewModel>> GetAllSalesOrders()
        {
            IEnumerable<SalesOrder> salesOrders = await _salesOrderRepository.GetAll();

            List<SalesOrderListViewModel> salesOrderViewModels = new List<SalesOrderListViewModel>();

            foreach (var salesOrder in salesOrders)
            {
                var customer = await _customerRepository.GetById(salesOrder.CustomerId);

                if (customer != null)
                {
                    SalesOrderListViewModel salesOrderViewModel = new SalesOrderListViewModel
                    {
                        Id = salesOrder.Id,
                        OrderNo = salesOrder.OrderNo,
                        OrderDate = salesOrder.OrderDate,
                        TotalAmount = salesOrder.TotalAmount,
                        CustomerName = customer.Name
                    };

                    salesOrderViewModels.Add(salesOrderViewModel);
                }
            }

            return salesOrderViewModels;
        }

        public async Task<SalesOrderDetailViewModel> GetSalesOrderById(Guid id)
        {
            SalesOrder salesOrder = await _salesOrderRepository.GetById(id);

            if (salesOrder == null)
            {
                throw new InvalidOperationException($"Sales Order with ID {id} not found.");
            }

            var customer = await _customerRepository.GetById(salesOrder.CustomerId);

            if (customer == null)
            {
                throw new InvalidOperationException($"Customer associated with Sales Order ID {id} not found.");
            }

            var customerItems = await _customerItemRepository.FindAll(ci => ci.SalesOrderId == id);
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

            var customerViewModel = new Domain.ViewModels.Customer.CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Contact = customer.Contact
            };

            SalesOrderDetailViewModel salesOrderDetailViewModel = new SalesOrderDetailViewModel
            {
                Id = salesOrder.Id,
                OrderNo = salesOrder.OrderNo,
                OrderDate = salesOrder.OrderDate,
                TotalAmount = salesOrder.TotalAmount,
                Customer = customerViewModel,
                CustomerItems = customerItemViewModels
            };

            return salesOrderDetailViewModel;
        }

        public async Task<SalesOrderViewModel> CreateSalesOrder(SalesOrderCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var customer = await _customerRepository.GetById(model.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {model.CustomerId} not found.");
            }

            string orderNo = $"SO-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";

            SalesOrder salesOrder = new SalesOrder
            {
                OrderNo = orderNo,
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                TotalAmount = 0,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            SalesOrder createdSalesOrder = await _salesOrderRepository.Add(salesOrder);

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

                    decimal gstAmount = (item.SellingPrice * item.GSTPercent) / 100;
                    decimal totalItemAmount = item.SellingPrice + gstAmount;

                    CustomerItem customerItem = new CustomerItem
                    {
                        ItemId = itemModel.ItemId,
                        CustomerId = model.CustomerId,
                        SalesOrderId = createdSalesOrder.Id,
                        GSTAmount = gstAmount,
                        TotalAmount = totalItemAmount,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };

                    await _customerItemRepository.Add(customerItem);

                    totalAmount += totalItemAmount;
                }
            }

            createdSalesOrder.TotalAmount = totalAmount;
            await _salesOrderRepository.Update(createdSalesOrder);

            var customerViewModel = new Domain.ViewModels.Customer.CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Contact = customer.Contact
            };

            SalesOrderViewModel salesOrderViewModel = new SalesOrderViewModel
            {
                Id = createdSalesOrder.Id,
                OrderNo = createdSalesOrder.OrderNo,
                OrderDate = createdSalesOrder.OrderDate,
                TotalAmount = createdSalesOrder.TotalAmount,
                Customer = customerViewModel
            };

            return salesOrderViewModel;
        }

        public async Task<bool> DeleteSalesOrder(Guid id)
        {
            SalesOrder salesOrder = await _salesOrderRepository.GetById(id);

            if (salesOrder == null)
            {
                return false;
            }

            var customerItems = await _customerItemRepository.FindAll(ci => ci.SalesOrderId == id);

            foreach (var customerItem in customerItems)
            {
                await _customerItemRepository.Delete(customerItem);
            }

            await _salesOrderRepository.Delete(salesOrder);

            return true;
        }

        public async Task<IEnumerable<SalesOrderListViewModel>> GetSalesOrdersByCustomer(Guid customerId)
        {
            Customer customer = await _customerRepository.GetById(customerId);

            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {customerId} not found.");
            }

            var salesOrders = await _salesOrderRepository.FindAll(so => so.CustomerId == customerId);

            List<SalesOrderListViewModel> salesOrderViewModels = new List<SalesOrderListViewModel>();

            foreach (var salesOrder in salesOrders)
            {
                SalesOrderListViewModel salesOrderViewModel = new SalesOrderListViewModel
                {
                    Id = salesOrder.Id,
                    OrderNo = salesOrder.OrderNo,
                    OrderDate = salesOrder.OrderDate,
                    TotalAmount = salesOrder.TotalAmount,
                    CustomerName = customer.Name
                };

                salesOrderViewModels.Add(salesOrderViewModel);
            }

            return salesOrderViewModels;
        }

        public async Task<IEnumerable<SalesOrderListViewModel>> SearchSalesOrders(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<SalesOrderListViewModel>();
            }

            string lowerSearchTerm = searchTerm.ToLower();

            IEnumerable<SalesOrder> salesOrders = await _salesOrderRepository.FindAll(so =>
                so.OrderNo.ToLower().Contains(lowerSearchTerm));

            List<SalesOrderListViewModel> salesOrderViewModels = new List<SalesOrderListViewModel>();

            foreach (var salesOrder in salesOrders)
            {
                var customer = await _customerRepository.GetById(salesOrder.CustomerId);

                if (customer != null)
                {
                    SalesOrderListViewModel salesOrderViewModel = new SalesOrderListViewModel
                    {
                        Id = salesOrder.Id,
                        OrderNo = salesOrder.OrderNo,
                        OrderDate = salesOrder.OrderDate,
                        TotalAmount = salesOrder.TotalAmount,
                        CustomerName = customer.Name
                    };

                    salesOrderViewModels.Add(salesOrderViewModel);
                }
            }
            return salesOrderViewModels;
        }
    }
}