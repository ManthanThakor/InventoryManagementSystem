using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Category;

namespace Application.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategories();
        Task<CategoryDetailViewModel?> GetCategoryById(Guid id);
        Task<CategoryViewModel> CreateCategory(CategoryCreateViewModel model);
        Task<CategoryViewModel> UpdateCategory(CategoryUpdateViewModel model);
        Task<bool> DeleteCategory(Guid id);
        Task<IEnumerable<CategoryViewModel>> SearchCategories(string searchTerm);
    }
}
