using BookedIn.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookedIn.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = _unitOfWork.ShoppingCart.GetAll(
                filter: u => u.ApplicationUserId == userId,
                "Product"
            ).ToList();

            return View(cartItems);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Remove(int id)
        {
            var itm=_unitOfWork.ShoppingCart.Get(u=>u.Id== id);
            if (itm == null)
            {
                return NotFound();
            }
            _unitOfWork.ShoppingCart.Remove(itm);
            _unitOfWork.Save();
            TempData["success"] = "Item removed from cart.";
            return RedirectToAction("Index");

        }
        [HttpPost]
        public IActionResult Increament(int cartId)
        {
            var cart=_unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cart == null)
            {
                return NotFound();
            }
            cart.Count += 1;
            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Decreament(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cart == null)
            {
                return NotFound();
            }
            if(cart.Count <= 1)
            {
                // If the count is 1 or less, remove the item instead of decrementing
                _unitOfWork.ShoppingCart.Remove(cart);
                TempData["success"] = "Item removed from cart.";
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cart);
            }
                _unitOfWork.Save();
                return RedirectToAction("Index");
           
        }

    }
}
