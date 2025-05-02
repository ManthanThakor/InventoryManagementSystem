using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{

    public interface INotificationService
    {
        Task NotifyAdminsAsync(object message);
        Task NotifyUserAsync(string userId, object message);
    }
}
