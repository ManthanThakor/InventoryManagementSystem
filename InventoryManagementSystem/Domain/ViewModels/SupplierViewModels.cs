using Domain.ViewModels.YourNamespace.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class SupplierRegisterViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact information is required")]
        [StringLength(50, ErrorMessage = "Contact information cannot exceed 50 characters")]
        [RegularExpression(@"^(\+\d{1,3})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$",
            ErrorMessage = "Please enter a valid contact number")]
        public string Contact { get; set; } = string.Empty;
    }

    public class SupplierViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public UserProfileViewModel? User { get; set; }
    }

    public class SupplierDetailViewModel : SupplierViewModel
    {
        public List<SupplierItemViewModel> SupplierItems { get; set; } = new List<SupplierItemViewModel>();
        public List<PurchaseOrderListViewModel> PurchaseOrders { get; set; } = new List<PurchaseOrderListViewModel>();
    }

    public class SupplierUpdateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact information is required")]
        [StringLength(50, ErrorMessage = "Contact information cannot exceed 50 characters")]
        [RegularExpression(@"^(\+\d{1,3})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$",
            ErrorMessage = "Please enter a valid contact number")]
        public string Contact { get; set; } = string.Empty;
    }
}
