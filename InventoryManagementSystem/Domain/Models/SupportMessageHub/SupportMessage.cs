using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.SupportMessageHub
{
    public class SupportMessage : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public string AdminResponse { get; set; } = string.Empty;
        public DateTime? ResponseDate { get; set; }

        public virtual User? User { get; set; }

    }
}
