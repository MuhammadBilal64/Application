using System.Diagnostics;
using System.Security.Claims;
using Application.Models;
using BookedIn.DataAccess.Repository;
using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.Models;
using BookedIn.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

namespace BookedIn.Areas.Custumer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
      //  private readonly ShoppingCart _shoppingCart;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index( int? page)
        {
            int pagesize = 8;
            int pageindex;
            if(page!=null && page > 0)
            {
                pageindex = Convert.ToInt32(page);
            }
            else
            {
                pageindex = 1;
            }

            var productList=_unitOfWork.Product.GetAll(includeproperties:"Category").
                ToPagedList(pageindex,pagesize);
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id, includeproperties: "Category");
            ProductDetailVM productDetailVM = new ProductDetailVM()
            {
                Product = product,
                Count=1,
                ProductId=product.Id,
            };
            return View(productDetailVM);
        }
        [HttpPost, ActionName("Details")]
        [Authorize]
      //  [ValidateAntiForgeryToken]
        public IActionResult Details(ProductDetailVM productDetailVM)
        {
          

            if (ModelState.IsValid)
            {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartfromdb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userid && u.ProductId == productDetailVM.ProductId);
                if (cartfromdb == null)
                {
                    ShoppingCart shoppingCart = new ShoppingCart()
                    {
                        ApplicationUserId = userid,
                        ProductId = productDetailVM.ProductId,
                        Count = productDetailVM.Count
                    };
                    _unitOfWork.ShoppingCart.Add(shoppingCart);

                }
                else
                {
                    cartfromdb.Count += productDetailVM.Count;
                    _unitOfWork.ShoppingCart.Update(cartfromdb);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product added to cart successfully!";
                return RedirectToAction("Index");
            }
            productDetailVM.Product = _unitOfWork.Product.Get(
       u => u.Id == productDetailVM.ProductId,
       includeproperties: "Category");

            return View(productDetailVM);
        }
       

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
