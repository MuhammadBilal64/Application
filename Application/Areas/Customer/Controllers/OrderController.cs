using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.Models;
using BookedIn.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookedIn.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork _unit)
        {
            _unitOfWork = _unit;
        }
        [Authorize] 

        public IActionResult Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = _unitOfWork.ShoppingCart.GetAll(
                filter: u => u.ApplicationUserId == userId,
                "Product"
            ).ToList();
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "You cannot access checkout after placing an order. Please add items to your cart again.";

                return RedirectToAction("Index", "Home");
            }
            var user= _unitOfWork.applicationUser.Get(u => u.Id == userId);
         
            OrderHeader orderHeader = new OrderHeader()
            {

                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                Name = user?.Name ?? "",
                PhoneNumber = user?.PhoneNumber ?? "",
                StreetAddress = user?.StreetAddress ?? "",
                City = user?.City ?? "",
                State = user?.State ?? "",
                PostalCode = user?.PostalCode ?? "",
                OrderTotal = (decimal)cartItems.Sum(u => u.Product.Price * u.Count)

            };
            OrderVM orderVM = new OrderVM
            {
                ShoppingCartList = cartItems,
                OrderHeader = orderHeader
            };

            return View(orderVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Checkout(OrderVM ordervm)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = _unitOfWork.ShoppingCart.GetAll(
                filter: u => u.ApplicationUserId == userid,
                "Product"
            ).ToList();

            if (cartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(ordervm);
            }

            if (ModelState.IsValid)
            {
                ordervm.OrderHeader.ApplicationUserId = userid;
                ordervm.OrderHeader.OrderDate = DateTime.Now;
                ordervm.OrderHeader.OrderTotal = (decimal)cartItems.Sum(u => u.Product.Price * u.Count);
                ordervm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ordervm.OrderHeader.OrderStatus = SD.OrderStatusPending;


                _unitOfWork.OrderHeader.Add(ordervm.OrderHeader);
                _unitOfWork.Save();

                foreach (var item in cartItems)
                {
                    OrderDetail detail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderHeaderId = ordervm.OrderHeader.Id,
                        Price = (decimal)item.Product.Price,
                        Count = item.Count
                    };
                    _unitOfWork.OrderDetail.Add(detail);
                }

                _unitOfWork.ShoppingCart.RemoveRange(cartItems);
                _unitOfWork.Save();

              //  TempData["success"] = "Order placed successfully.";
                return RedirectToAction("PaymentMethod", new { id = ordervm.OrderHeader.Id });
            }

            // If ModelState is invalid
            ordervm.ShoppingCartList = cartItems;
            return View(ordervm);
        }
        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id,  "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id,"Product").ToList();

            OrderVM orderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                ShoppingCartList = orderDetails.Select(od => new ShoppingCart
                {
                    Product = od.Product,
                    Count = od.Count,
                    ProductId = od.ProductId
                }).ToList()
            };

            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayOnDelivery(int id)
        {


            var order=_unitOfWork.OrderHeader.Get(u => u.Id == id, "ApplicationUser");
            if (order == null)
            {
                return NotFound();
            }
            if (order.PaymentStatus != SD.PaymentStatusPending)
            {
                TempData["error"] = "This order has already been paid for.";
                return RedirectToAction("OrderConfirmation", new { id = id });
            }
            order.PaymentStatus=SD.PaymentStatusCOD;
            order.OrderStatus =SD.OrderStatusPlaced;
            _unitOfWork.OrderHeader.Update(order);
            _unitOfWork.Save();
            TempData["success"] = "Order placed successfully.";
            return RedirectToAction("OrderConfirmation", new { id = id });
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
       
        //public IActionResult PayNow(int id)
        //{
        //    var order = _unitOfWork.OrderHeader.Get(u => u.Id == id, "ApplicationUser");
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }
        //    if(order.PaymentStatus != SD.PaymentStatusPending)
        //    {
        //        TempData["error"] = "This order has already been paid for.";
        //        return RedirectToAction("OrderConfirmation", new { id = id });
        //    }
        //    order.PaymentStatus = SD.PaymentStatusPaid;
        //    order.OrderStatus = SD.OrderStatusPlaced;
        //    _unitOfWork.OrderHeader.Update(order);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Order placed successfully.";
        //    return RedirectToAction("OrderConfirmation", new { id = id });
        //}

        public IActionResult PaymentMethod(int id)
        {
            var order = _unitOfWork.OrderHeader.Get(u => u.Id == id, "ApplicationUser");
            if (order == null || order.PaymentStatus != SD.PaymentStatusPending)
            {
                TempData["error"] = "Invalid or already processed order.";
                return RedirectToAction("OrderConfirmation", new { id = id });
            }
            var orderVM = new OrderVM
            {
                OrderHeader = order
            };

            return View(orderVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStripeSession(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId,  "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, "Product").ToList();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Order/PaymentConfirmation?orderId={orderId}",
                CancelUrl = domain + $"Customer/Order/PaymentMethod/{orderId}",
            };

            foreach (var item in orderDetails)
            {
                var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), 
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };

                options.LineItems.Add(sessionLineItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            // Store Stripe session ID in the order (optional but useful)

            orderHeader.SessionId = session.Id;
            orderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            // Redirect to Stripe Checkout
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult PaymentConfirmation(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, "ApplicationUser");

            if (orderHeader == null)
                return NotFound();

            if (orderHeader.PaymentStatus == SD.PaymentStatusPaid)
            {
                // Already paid
                return RedirectToAction("OrderConfirmation", new { id = orderId });
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus == "paid")
            {
                orderHeader.PaymentStatus = SD.PaymentStatusPaid;
                orderHeader.OrderStatus = SD.OrderStatusPlaced;
                _unitOfWork.OrderHeader.Update(orderHeader);
                _unitOfWork.Save();
            }

            return RedirectToAction("OrderConfirmation", new { id = orderId });
        }


    }
}
