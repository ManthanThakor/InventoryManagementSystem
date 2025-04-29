using Domain.ViewModels.Orders;

namespace Application.Services.SalesOrderServices
{
    public interface ISalesOrderService
    {
        Task<IEnumerable<SalesOrderListViewModel>> GetAllSalesOrders();
        Task<SalesOrderDetailViewModel> GetSalesOrderById(Guid id);
        Task<SalesOrderViewModel> CreateSalesOrder(SalesOrderCreateViewModel model);
        Task<bool> DeleteSalesOrder(Guid id);
        Task<IEnumerable<SalesOrderListViewModel>> GetSalesOrdersByCustomer(Guid customerId);
        Task<IEnumerable<SalesOrderListViewModel>> SearchSalesOrders(string searchTerm);
    }
}
