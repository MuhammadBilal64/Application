using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataAccess.Data;
using Application.DataAccess.IRepository;
using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.Models;

namespace Application.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProduct
    {
        private AppsContext _Content;
        public ProductRepository(AppsContext content) : base(content)
        {
            _Content = content ?? throw new ArgumentNullException(nameof(content));
        }

      

        public void Update(Product obj)
        {
           
           var objfromDb=_Content.Products.FirstOrDefault(u=>u.Id==obj.Id);
            if (objfromDb != null)
            {
                objfromDb.Title = obj.Title;
                objfromDb.ISBN = obj.ISBN;
                objfromDb.Price = obj.Price;
                objfromDb.Price50 = obj.Price50;
                objfromDb.ListPrice = obj.ListPrice;
                objfromDb.Price100 = obj.Price100;
                objfromDb.Description = obj.Description;
                objfromDb.Author = obj.Author;
                objfromDb.CategoryId = obj.CategoryId;
                if (obj.ImageUrl != null)
                {
                    objfromDb.ImageUrl= obj.ImageUrl;
                }
            }
        }
    }
}
