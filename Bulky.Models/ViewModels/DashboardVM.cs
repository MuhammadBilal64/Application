using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public decimal TotalRevenue { get; set; } 
        public int TotalPendingOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
      

    }
}
