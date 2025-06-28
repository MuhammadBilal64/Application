using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.DataAccess;
using Application.DataAccess.Data;
using Application.DataAccess.Repository;
using BookedIn.Models;

namespace BookedIn.DataAccess.Repository
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly AppsContext _context;
        public ICategory Category { get; private set; }
        public IProduct Product { get; private set; }
     public   IShoppingCart ShoppingCart { get; }
        // Add other repositories as needed, e.g. IOrderDetail, IOrderHeader, etc.
         public IOrderDetail OrderDetail { get; private set; }
         public IOrderheader OrderHeader { get; private set; }
            public IApplicationUser applicationUser { get; private set; }

        public UnitOfWork(AppsContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Product = new ProductRepository(_context);
            ShoppingCart = new ShoppingCartRepo(_context);
            OrderDetail = new OrderDetailRepo(_context);
            OrderHeader = new OrderHeaderRepo(_context);
            applicationUser = new ApplicationUserRepo(_context);
        }

        public void Save()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }
            _context.SaveChanges();
        }
    }

}
