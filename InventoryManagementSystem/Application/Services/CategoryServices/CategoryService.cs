using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Category;
using Domain.ViewModels.Item;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategories()
        {
            IEnumerable<Category> categories = await _categoryRepository.GetAll();

            List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();

            foreach (Category category in categories)
            {
                if (category != null)
                {
                    CategoryViewModel categoryViewMod = new CategoryViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    };

                    categoryViewModels.Add(categoryViewMod);
                }
            }
            return categoryViewModels;
        }

        public async Task<CategoryDetailViewModel> GetCategoryById(Guid id)
        {
            Category category = await _categoryRepository.GetById(id);

            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {id} not found.");
            }

            CategoryDetailViewModel categoryDetailViewModel = new CategoryDetailViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedDate = category.CreatedDate,
                ModifiedDate = category.ModifiedDate,
            };

            return categoryDetailViewModel;
        }

        public async Task<CategoryViewModel> CreateCategory(CategoryCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Category category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Category createdCategory = await _categoryRepository.Add(category);

            if (createdCategory == null)
            {
                throw new InvalidOperationException("Failed to create category.");
            }

            CategoryViewModel categoryViewModel = new CategoryViewModel
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                Description = createdCategory.Description
            };

            return categoryViewModel;
        }

        public async Task<CategoryViewModel> UpdateCategory(CategoryUpdateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Category category = await _categoryRepository.GetById(model.Id);

            if (category == null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.ModifiedDate = DateTime.UtcNow;

            await _categoryRepository.Update(category);

            CategoryViewModel updatedCategoryViewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return updatedCategoryViewModel;
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            Category category = await _categoryRepository.GetById(id);

            if (category == null)
            {
                return false;
            }

            await _categoryRepository.Delete(category);

            return true;
        }

        public async Task<IEnumerable<CategoryViewModel>> SearchCategories(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<CategoryViewModel>();
            }

            string lowerSearchTerm = searchTerm.ToLower();

            IEnumerable<Category> categories = await _categoryRepository.FindAll(c => c.Name.ToLower().Contains(lowerSearchTerm));

            List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();

            foreach (Category category in categories)
            {
                if (category != null)
                {
                    CategoryViewModel categoryViewModel = new CategoryViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    };

                    categoryViewModels.Add(categoryViewModel);
                }
            }
            return categoryViewModels;
        }
    }
}
