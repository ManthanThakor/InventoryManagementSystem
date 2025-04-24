using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public SupplierViewModel? Supplier { get; set; }
    }

    public class PurchaseOrderDetailViewModel : PurchaseOrderViewModel
    {
        public List<SupplierItemViewModel> SupplierItems { get; set; } = new List<SupplierItemViewModel>();
    }

    public class PurchaseOrderListViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    public class PurchaseOrderCreateViewModel
    {
        [Required(ErrorMessage = "Supplier is required")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "At least one item is required")]
        public List<SupplierItemCreateViewModel> Items { get; set; } = new List<SupplierItemCreateViewModel>();
    }

    public class SalesOrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public CustomerViewModel? Customer { get; set; }
    }

    public class SalesOrderDetailViewModel : SalesOrderViewModel
    {
        public List<CustomerItemViewModel> CustomerItems { get; set; } = new List<CustomerItemViewModel>();
    }

    public class SalesOrderListViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }

    public class SalesOrderCreateViewModel
    {
        [Required(ErrorMessage = "Customer is required")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "At least one item is required")]
        public List<CustomerItemCreateViewModel> Items { get; set; } = new List<CustomerItemCreateViewModel>();
    }
}
