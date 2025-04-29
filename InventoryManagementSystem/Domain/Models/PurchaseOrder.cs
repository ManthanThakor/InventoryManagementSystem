using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PurchaseOrder : BaseEntity
    {
        public string OrderNo { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual Supplier? Supplier { get; set; }
        [JsonIgnore]
        public virtual ICollection<SupplierItem> SupplierItems { get; set; } = new List<SupplierItem>();
    }

}
