using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Orders;

namespace Application.Services.PurchaseOrderServices
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrderListViewModel>> GetAllPurchaseOrders();
        Task<PurchaseOrderDetailViewModel> GetPurchaseOrderById(Guid id);
        Task<PurchaseOrderViewModel> CreatePurchaseOrder(PurchaseOrderCreateViewModel model);
        Task<bool> DeletePurchaseOrder(Guid id);
        Task<IEnumerable<PurchaseOrderListViewModel>> GetPurchaseOrdersBySupplier(Guid supplierId);
        Task<IEnumerable<PurchaseOrderListViewModel>> SearchPurchaseOrders(string searchTerm);
    }
}
