using Application.DataAccess.Data;
using Application.Models;
using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.Models;
using BookedIn.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BookedIn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment hostingEnvironment)
        {
            _unitofwork = unitofwork;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objlist = _unitofwork.Product.GetAll(includeproperties:"Category").ToList();
          

            return View(objlist);
        }

        public IActionResult Upsert(int ?id)
        {
            ProductVM productvm = new()
            {
                CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null||id==0)
            {
                return View(productvm);
            }
            //update
            else
            {
                productvm.Product = _unitofwork.Product.Get(u => u.Id == id);
                if (productvm.Product == null)
                {
                    return NotFound();
                }
                return View(productvm);

            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        { 
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostingEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename= Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    var productpath = Path.Combine(wwwRootPath, @"Images\product");
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldimage_path = Path.Combine(wwwRootPath,obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimage_path))
                        {
                            System.IO.File.Delete(oldimage_path);


                        }

                    }

                    using (var fileStream = new FileStream(Path.Combine(productpath, filename),FileMode.Create))
                    {
                           file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\Images\product\" + filename;


                }
                if (obj.Product.Id == 0)
                {
                _unitofwork.Product.Add(obj.Product);
                    TempData["success"] = "Product Created Successfully";

                }
                else
                {
                    _unitofwork.Product.Update(obj.Product);
                    TempData["success"] = "Product Updated Successfully";

                }


                _unitofwork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitofwork.Category.GetAll().Select(u =>new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                );
            }
            return View(obj);
        }

     
        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objlist = _unitofwork.Product.GetAll(includeproperties: "Category").ToList();
            return Json(new { data = objlist });


        }
        [HttpDelete]
        public IActionResult Delete(int ?id)

        {
          var  productTobeDeleted = _unitofwork.Product.Get(u => u.Id == id);
            if (productTobeDeleted == null)
            {
                return Json(new { success = false,message="Error While Deleting" });
            }
            var oldimage_path = Path.Combine(_hostingEnvironment.WebRootPath,productTobeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldimage_path))
            {
                System.IO.File.Delete(oldimage_path);


            }
            _unitofwork.Product.Remove(productTobeDeleted);
            _unitofwork.Save();
            return Json(new { success=true,message="Delete Successfull" });
        }



        #endregion

    }

}