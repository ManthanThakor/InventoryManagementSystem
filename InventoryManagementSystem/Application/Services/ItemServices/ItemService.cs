using Application.Services.GeneralServices;
using Domain.Models;
using Domain.ViewModels.Category;
using Domain.ViewModels.Item;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.ItemServices
{
    public class ItemService : IItemService
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ItemService(
            IRepository<Item> itemRepository,
            IRepository<Category> categoryRepository)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<ItemViewModel>> GetAllItems()
        {
            IEnumerable<Item> items = await _itemRepository.FindAll(i => true);

            List<ItemViewModel> itemsWithCategory = new List<ItemViewModel>();
            foreach (Item item in items)
            {
                ItemViewModel itemViewModel = new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    GSTPercent = item.GSTPercent,
                    PurchasePrice = item.PurchasePrice,
                    SellingPrice = item.SellingPrice
                };

                if (item.Category != null)
                {
                    itemViewModel.Category = new CategoryViewModel
                    {
                        Id = item.Category.Id,
                        Name = item.Category.Name,
                        Description = item.Category.Description
                    };
                }
                else
                {
                    itemViewModel.Category = null;
                }

                itemsWithCategory.Add(itemViewModel);
            }

            return itemsWithCategory;
        }

        public async Task<ItemDetailViewModel> GetItemById(Guid id)
        {
            Item item = await _itemRepository.GetById(id);

            if (item == null)
            {
                throw new InvalidOperationException($"Item with ID {id} not found.");
            }

            var category = await _categoryRepository.GetById(item.CategoryId);

            ItemDetailViewModel itemViewModel = new ItemDetailViewModel
            {
                Id = item.Id,
                Name = item.Name,
                GSTPercent = item.GSTPercent,
                PurchasePrice = item.PurchasePrice,
                SellingPrice = item.SellingPrice,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate
            };

            if (category != null)
            {
                itemViewModel.Category = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };
            }
            else
            {
                itemViewModel.Category = null;
            }

            return itemViewModel;
        }

        public async Task<ItemViewModel> CreateItem(ItemCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Category category = await _categoryRepository.GetById(model.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {model.CategoryId} not found.");
            }

            Item item = new Item
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                GSTPercent = model.GSTPercent,
                PurchasePrice = model.PurchasePrice,
                SellingPrice = model.SellingPrice,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Item createdItem = await _itemRepository.Add(item);

            ItemViewModel itemViewModel = new ItemViewModel
            {
                Id = createdItem.Id,
                Name = createdItem.Name,
                GSTPercent = createdItem.GSTPercent,
                PurchasePrice = createdItem.PurchasePrice,
                SellingPrice = createdItem.SellingPrice,
                Category = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            };

            return itemViewModel;
        }

        public async Task<ItemViewModel> UpdateItem(ItemUpdateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Item item = await _itemRepository.GetById(model.Id);

            if (item == null)
            {
                throw new InvalidOperationException($"Item with ID {model.Id} not found.");
            }

            var category = await _categoryRepository.GetById(model.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {model.CategoryId} not found.");
            }

            item.Name = model.Name;
            item.CategoryId = model.CategoryId;
            item.GSTPercent = model.GSTPercent;
            item.PurchasePrice = model.PurchasePrice;
            item.SellingPrice = model.SellingPrice;
            item.ModifiedDate = DateTime.UtcNow;

            await _itemRepository.Update(item);

            var itemViewModel = new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                GSTPercent = item.GSTPercent,
                PurchasePrice = item.PurchasePrice,
                SellingPrice = item.SellingPrice,
                Category = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            };

            return itemViewModel;
        }

        public async Task<bool> DeleteItem(Guid id)
        {
            Item item = await _itemRepository.GetById(id);

            if (item == null)
            {
                return false;
            }

            await _itemRepository.Delete(item);
            return true;
        }

        public async Task<IEnumerable<ItemViewModel>> GetItemsByCategory(Guid categoryId)
        {
            var category = await _categoryRepository.GetById(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            IEnumerable<Item> items = await _itemRepository.FindAll(i => i.CategoryId == categoryId);

            var itemViewModels = new List<ItemViewModel>();
            foreach (var item in items)
            {
                var viewModel = new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    GSTPercent = item.GSTPercent,
                    PurchasePrice = item.PurchasePrice,
                    SellingPrice = item.SellingPrice,
                    Category = new CategoryViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    }
                };

                itemViewModels.Add(viewModel);
            }

            return itemViewModels;
        }

        public async Task<IEnumerable<ItemViewModel>> SearchItems(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<ItemViewModel>();
            }

            string lowerSearchTerm = searchTerm.ToLower();

            IEnumerable<Item> items = await _itemRepository.FindAll(i =>
                i.Name.ToLower().Contains(lowerSearchTerm));

            var itemViewModels = new List<ItemViewModel>();

            foreach (var item in items)
            {
                var category = await _categoryRepository.GetById(item.CategoryId);

                var viewModel = new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    GSTPercent = item.GSTPercent,
                    PurchasePrice = item.PurchasePrice,
                    SellingPrice = item.SellingPrice
                };

                if (category != null)
                {
                    viewModel.Category = new CategoryViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    };
                }
                else
                {
                    viewModel.Category = null;
                }

                itemViewModels.Add(viewModel);
            }

            return itemViewModels;
        }
    }
}
