using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CustomerItem : BaseEntity
    {
        public Guid SalesOrderId { get; set; }
        public Guid ItemId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual SalesOrder? SalesOrder { get; set; }
        public virtual Item? Item { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
