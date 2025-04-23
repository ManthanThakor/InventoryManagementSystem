using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SalesOrder : BaseEntity
    {
        public string OrderNo { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual Customer? Customer { get; set; }
        [JsonIgnore]
        public virtual ICollection<CustomerItem> CustomerItems { get; set; } = new List<CustomerItem>();
    }
}
