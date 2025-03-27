using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataAccess.Data;
using Application.DataAccess.IRepository;
using BookedIn.DataAccess.Repository.IRepository;

namespace Application.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategory
    {
        private AppsContext _Content;
        public CategoryRepository(AppsContext content) : base(content)
        {
            _Content = content ?? throw new ArgumentNullException(nameof(content));
        }

      

        public void Update(Category obj)
        {
            if (_Content == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }
            _Content.Categories.Update(obj);
        }
    }
}
