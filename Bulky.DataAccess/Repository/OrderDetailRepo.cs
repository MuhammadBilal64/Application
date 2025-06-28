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
    public class OrderDetailRepo : Repository<OrderDetail>, IOrderDetail
    {
     private AppsContext _Content;

        public OrderDetailRepo(AppsContext content) : base(content)
        {
            _Content = content;
        }

        public void Update(OrderDetail obj)
        {
          _Content.Update(obj);
        }
    }
}
