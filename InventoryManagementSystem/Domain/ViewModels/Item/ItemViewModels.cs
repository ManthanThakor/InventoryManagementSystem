using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Category;

namespace Domain.ViewModels.Item
{
    public class ItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal GSTPercent { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public CategoryViewModel? Category { get; set; }
    }

    public class ItemDetailViewModel : ItemViewModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class ItemCreateViewModel
    {
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "GST percent is required")]
        [Range(0, 100, ErrorMessage = "GST percent must be between 0 and 100")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal GSTPercent { get; set; }

        [Required(ErrorMessage = "Purchase price is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Purchase price must be greater than 0")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "Selling price is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Selling price must be greater than 0")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal SellingPrice { get; set; }
    }

    public class ItemUpdateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "GST percent is required")]
        [Range(0, 100, ErrorMessage = "GST percent must be between 0 and 100")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal GSTPercent { get; set; }

        [Required(ErrorMessage = "Purchase price is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Purchase price must be greater than 0")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "Selling price is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Selling price must be greater than 0")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal SellingPrice { get; set; }
    }
}
