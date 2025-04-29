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
        [Required(ErrorMessage = "Supplier ID is required")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "Order Date is required")]
        public DateTime OrderDate { get; set; }

        public List<PurchaseOrderItemCreateViewModel> Items { get; set; } = new List<PurchaseOrderItemCreateViewModel>();
    }

    public class PurchaseOrderItemCreateViewModel
    {
        [Required(ErrorMessage = "Item ID is required")]
        public Guid ItemId { get; set; }
    }

}
