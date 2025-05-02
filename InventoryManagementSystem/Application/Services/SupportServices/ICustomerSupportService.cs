using Domain.ViewModels.SupportViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.SupportServices
{
    public interface ICustomerSupportService
    {
        Task<SupportMessageViewModel> SaveSupportMessage(SupportMessageCreateViewModel messageViewModel);
        Task<IEnumerable<SupportMessageViewModel>> GetUserSupportHistory(Guid userId);
        Task<IEnumerable<SupportMessageViewModel>> GetAllPendingSupportMessages();
        Task<SupportMessageViewModel> RespondToSupportMessage(SupportResponseViewModel responseViewModel);
    }
}
