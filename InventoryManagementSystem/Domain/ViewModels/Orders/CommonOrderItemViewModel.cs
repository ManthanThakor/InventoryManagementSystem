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
        public int Quantity { get; set; }
    }

    public class SupplierItemCreateViewModel
    {
        [Required(ErrorMessage = "Item is required")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class CustomerItemViewModel
    {
        public Guid Id { get; set; }
        public ItemViewModel? Item { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
    }

    public class CustomerItemCreateViewModel
    {
        [Required(ErrorMessage = "Item is required")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
