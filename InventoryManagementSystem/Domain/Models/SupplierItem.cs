using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SupplierItem : BaseEntity
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid ItemId { get; set; }
        public Guid SupplierId { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual PurchaseOrder? PurchaseOrder { get; set; }
        public virtual Item? Item { get; set; }
        public virtual Supplier? Supplier { get; set; }
    }
}
