using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ViewModels.Customer;

namespace Domain.ViewModels.Orders
{
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
