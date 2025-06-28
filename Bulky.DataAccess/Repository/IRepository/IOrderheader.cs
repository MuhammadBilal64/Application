using BookedIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.DataAccess.Repository.IRepository
{
   public interface IOrderheader:IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

    }
}
