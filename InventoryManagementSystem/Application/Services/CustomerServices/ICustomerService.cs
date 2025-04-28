using Domain.ViewModels.Customer;
using Domain.ViewModels.Orders;

namespace Application.Services.CustomerServices
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomers();
        Task<CustomerDetailViewModel> GetCustomerById(Guid id);
        Task<CustomerViewModel> UpdateCustomer(CustomerUpdateViewModel model);
        Task<bool> DeleteCustomer(Guid id);
        Task<IEnumerable<CustomerItemViewModel>> GetItemsByCustomer(Guid customerId);
        Task<CustomerItemViewModel> AddCustomerItem(CustomerItemCreateViewModel model);
        Task<bool> RemoveCustomerItem(Guid customerItemId);
        Task<IEnumerable<CustomerViewModel>> SearchCustomers(string searchTerm);
    }
}
