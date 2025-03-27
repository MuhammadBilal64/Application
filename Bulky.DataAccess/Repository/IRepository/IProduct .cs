using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookedIn.Models;

namespace BookedIn.DataAccess.Repository.IRepository
{
    public interface IProduct : IRepository<Product>
    {
        void Update(Product obj);
       
    }
}
