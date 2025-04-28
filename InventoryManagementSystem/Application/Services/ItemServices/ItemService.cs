using Application.Services.GeneralServices;
using Domain.Models;
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
            var itemsWithCategory = items.Select(i => new ItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                GSTPercent = i.GSTPercent,
                PurchasePrice = i.PurchasePrice,
                SellingPrice = i.SellingPrice,
                Category = i.Category != null ? new Domain.ViewModels.Category.CategoryViewModel
                {
                    Id = i.Category.Id,
                    Name = i.Category.Name,
                    Description = i.Category.Description
                } : null
            });

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
                ModifiedDate = item.ModifiedDate,
                Category = category != null ? new Domain.ViewModels.Category.CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                } : null
            };

            return itemViewModel;
        }

        public async Task<ItemViewModel> CreateItem(ItemCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Verify category exists
            var category = await _categoryRepository.GetById(model.CategoryId);
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
                Category = new Domain.ViewModels.Category.CategoryViewModel
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

            // Verify category exists
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

            ItemViewModel itemViewModel = new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                GSTPercent = item.GSTPercent,
                PurchasePrice = item.PurchasePrice,
                SellingPrice = item.SellingPrice,
                Category = new Domain.ViewModels.Category.CategoryViewModel
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
            // Verify category exists
            var category = await _categoryRepository.GetById(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            IEnumerable<Item> items = await _itemRepository.FindAll(i => i.CategoryId == categoryId);

            var itemViewModels = items.Select(i => new ItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                GSTPercent = i.GSTPercent,
                PurchasePrice = i.PurchasePrice,
                SellingPrice = i.SellingPrice,
                Category = new Domain.ViewModels.Category.CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            });

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

                itemViewModels.Add(new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    GSTPercent = item.GSTPercent,
                    PurchasePrice = item.PurchasePrice,
                    SellingPrice = item.SellingPrice,
                    Category = category != null ? new Domain.ViewModels.Category.CategoryViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    } : null
                });
            }

            return itemViewModels;
        }
    }
}