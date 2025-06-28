using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.Models;
using BookedIn.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookedIn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee,Company")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var orders = _unitOfWork.OrderHeader.GetAll(includeproperties: "ApplicationUser");
            return View(orders);
        }
        public IActionResult Update(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, "ApplicationUser");
            if (orderHeader == null)
            {
                return NotFound();
            }

            return View(orderHeader);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(OrderHeader orderHeader)
        {
            var orderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeader.Id);
            if (orderFromDb == null)
            {
                return NotFound();
            }

            // Only update fields admin is allowed to change
            orderFromDb.Name = orderHeader.Name;
            orderFromDb.PhoneNumber = orderHeader.PhoneNumber;
            orderFromDb.StreetAddress = orderHeader.StreetAddress;
            orderFromDb.City = orderHeader.City;
            orderFromDb.State = orderHeader.State;
            orderFromDb.PostalCode = orderHeader.PostalCode;

            orderFromDb.OrderStatus = orderHeader.OrderStatus;
            orderFromDb.PaymentStatus = orderHeader.PaymentStatus;

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order updated successfully";
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id, "Product");

            if (orderHeader == null || orderDetails == null || !orderDetails.Any())
                return NotFound();

            OrderDetailsVM viewModel = new OrderDetailsVM
            {
                OrderHeader = orderHeader,
                OrderDetailList = orderDetails
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var orderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == id);
            if (orderFromDb == null)
            {
                return NotFound();
            }

            // Check if the order is already canceled
            if (orderFromDb.OrderStatus == "Cancelled")
            {
                TempData["error"] = "Order is already canceled.";
                return RedirectToAction("Index");
            }
            // Soft delete approach - update status to Canceled
            orderFromDb.OrderStatus = "Cancelled";
            orderFromDb.PaymentStatus = "pending"; // Assuming you want to cancel payment status as well
            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order canceled successfully.";
            return RedirectToAction("Index");
        }



    }
}
