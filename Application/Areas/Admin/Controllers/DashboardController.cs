using BookedIn.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookedIn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class DashboardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public DashboardController( IUnitOfWork unit)
        {
            _unitOfWork = unit;
        }
        public IActionResult Index()
        {
            var orders = _unitOfWork.OrderHeader.GetAll();
            var users= _unitOfWork.applicationUser.GetAll();
            var products = _unitOfWork.Product.GetAll();
            var categories = _unitOfWork.Category.GetAll();
            var dashboardVM = new Models.ViewModels.DashboardVM
            {
                TotalOrders = orders.Count(),
                TotalUsers = users.Count(),
                TotalProducts = products.Count(),
                TotalCategories = categories.Count(),
                TotalRevenue = orders.Sum(o => o.OrderTotal),
                TotalPendingOrders = orders.Count(o => o.OrderStatus == SD.OrderStatusPending),
                TotalCompletedOrders = orders.Count(o => o.OrderStatus == SD.OrderStatusPlaced),
                TotalCancelledOrders = orders.Count(o => o.OrderStatus == SD.OrderStatusCancelled)
            };

            return View(dashboardVM);
        }
    }
}
