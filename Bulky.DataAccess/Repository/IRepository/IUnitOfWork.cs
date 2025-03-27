using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
     ICategory Category { get; }

        IProduct Product { get; }


        void Save();
        
           
    }
}
