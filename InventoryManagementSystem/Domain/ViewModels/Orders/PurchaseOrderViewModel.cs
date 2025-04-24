using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Supplier;

namespace Domain.ViewModels.Orders
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
}
