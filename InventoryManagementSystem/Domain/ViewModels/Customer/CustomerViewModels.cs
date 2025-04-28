using Domain.ViewModels.Authentication;
using Domain.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.Customer
{
    public class CustomerRegisterViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Customer name is required")]
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

    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public UserProfileViewModel? User { get; set; }
    }

    public class  CustomerDetailViewModel : CustomerViewModel
    {
        public List<CustomerItemViewModel> CustomerItems { get; set; } = new List<CustomerItemViewModel>();
        public List<SalesOrderListViewModel> SalesOrders { get; set; } = new List<SalesOrderListViewModel>();

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class CustomerUpdateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
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