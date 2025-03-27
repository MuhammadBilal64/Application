using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.DataAccess;
using Application.DataAccess.Data;
using Application.DataAccess.Repository;

namespace BookedIn.DataAccess.Repository
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly AppsContext _context;
        public ICategory Category { get; private set; }
        public IProduct Product { get; private set; }


        public UnitOfWork(AppsContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Product = new ProductRepository(_context);
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
