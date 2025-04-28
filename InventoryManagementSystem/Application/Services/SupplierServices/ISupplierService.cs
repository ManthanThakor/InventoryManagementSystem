using Domain.ViewModels.Orders;
using Domain.ViewModels.Supplier;

namespace Application.Services.SupplierServices
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierViewModel>> GetAllSuppliers();
        Task<SupplierDetailViewModel> GetSupplierById(Guid id);
        Task<SupplierViewModel> UpdateSupplier(SupplierUpdateViewModel model);
        Task<bool> DeleteSupplier(Guid id);
        Task<IEnumerable<SupplierItemViewModel>> GetItemsBySupplier(Guid supplierId);
        Task<SupplierItemViewModel> AddSupplierItem(SupplierItemCreateViewModel model);
        Task<bool> RemoveSupplierItem(Guid supplierItemId);
        Task<IEnumerable<SupplierViewModel>> SearchSuppliers(string searchTerm);
    }
}
