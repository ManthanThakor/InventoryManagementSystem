using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public int CustomersCount { get; set; }
        public int SuppliersCount { get; set; }
        public int CategoriesCount { get; set; }
        public int ItemsCount { get; set; }
        public decimal SalesTotal { get; set; }
        public int SalesCount { get; set; }
        public decimal PurchaseTotal { get; set; }
        public int PurchaseCount { get; set; }
        public List<RecentOrderViewModel> RecentSales { get; set; } = new List<RecentOrderViewModel>();
        public List<RecentOrderViewModel> RecentPurchases { get; set; } = new List<RecentOrderViewModel>();
    }

    public class RecentOrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
