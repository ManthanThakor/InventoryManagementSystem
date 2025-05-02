using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.SupportViewModels
{
    public class SupportMessageViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsResolved { get; set; }
        public string AdminResponse { get; set; } = string.Empty;
        public DateTime? ResponseDate { get; set; }
    }

    public class SupportMessageCreateViewModel
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class SupportResponseViewModel
    {
        public Guid MessageId { get; set; }
        public string Response { get; set; } = string.Empty;
        public Guid AdminId { get; set; }
    }

    public class SupportStatisticsViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public double AverageResponseTime { get; set; }
    }
}
