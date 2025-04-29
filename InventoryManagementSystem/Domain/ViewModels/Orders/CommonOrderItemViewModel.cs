using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Item;

namespace Domain.ViewModels.Orders
{
    public class SupplierItemViewModel
    {
        public Guid Id { get; set; }
        public ItemViewModel? Item { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SupplierItemCreateViewModel
    {
        [Required(ErrorMessage = "Item is required")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Supplier ID is required")]
        public Guid SupplierId { get; set; }

        //[Required(ErrorMessage = "GST Amount is required")]
        //public decimal GSTAmount { get; set; }

        //[Required(ErrorMessage = "Total Amount is required")]
        //public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Purchase Order ID is required")]
        public Guid PurchaseOrderId { get; set; }
    }

    public class CustomerItemViewModel
    {
        public Guid Id { get; set; }
        public ItemViewModel? Item { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }


    public class CustomerItemCreateViewModel
    {
        [Required(ErrorMessage = "Item is required")]
        public Guid ItemId { get; set; }

        //[Required(ErrorMessage = "GST Amount is required")]
        //public decimal GSTAmount { get; set; }

        //[Required(ErrorMessage = "Total Amount is required")]
        //public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Sales Order ID is required")]
        public Guid SalesOrderId { get; set; }
    }
}
