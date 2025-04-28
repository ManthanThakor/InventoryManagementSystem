using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Item;

namespace Application.Services.ItemServices
{
    public interface IItemService
    {
        Task<IEnumerable<ItemViewModel>> GetAllItems();
        Task<ItemDetailViewModel> GetItemById(Guid id);
        Task<ItemViewModel> CreateItem(ItemCreateViewModel model);
        Task<ItemViewModel> UpdateItem(ItemUpdateViewModel model);
        Task<bool> DeleteItem(Guid id);
        Task<IEnumerable<ItemViewModel>> GetItemsByCategory(Guid categoryId);
        Task<IEnumerable<ItemViewModel>> SearchItems(string searchTerm);
    }
}
