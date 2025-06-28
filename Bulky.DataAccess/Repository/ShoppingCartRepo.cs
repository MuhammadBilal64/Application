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
    public class ShoppingCartRepo : Repository<ShoppingCart>, IShoppingCart
    {
        private AppsContext _Content;

        public ShoppingCartRepo(AppsContext content) : base(content)
        {
            _Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public void Update(ShoppingCart obj)
        {
         _Content.ShoppingCarts.Update(obj);
        }
    }
}
