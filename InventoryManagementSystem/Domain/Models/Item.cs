using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }

        public virtual Category? Category { get; set; }
        [JsonIgnore]
        public virtual SupplierItem? SupplierItem { get; set; }
        [JsonIgnore]
        public virtual CustomerItem? CustomerItem { get; set; }
    }
}
