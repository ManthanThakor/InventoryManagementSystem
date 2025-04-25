using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Category;

namespace Application.Services.CategoryServices
{
    public interface ICategoryService : IService<Category>
    {
        Task<CategoryDetailViewModel> GetCategoryWithItems(Guid categoryId);
    }
}
