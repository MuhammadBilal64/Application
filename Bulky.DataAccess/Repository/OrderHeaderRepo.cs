using Application.DataAccess.Data;
using Application.DataAccess.IRepository;
using BookedIn.DataAccess.Repository.IRepository;
using BookedIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.DataAccess.Repository
{
   public class OrderHeaderRepo : Repository<OrderHeader>, IOrderheader
    {
        private AppsContext _Content;

        public OrderHeaderRepo(AppsContext content) : base(content)
        {
            _Content = content;
        }


        public void Update(OrderHeader obj)
        {
            _Content.Update(obj); 
        }
    }
}
