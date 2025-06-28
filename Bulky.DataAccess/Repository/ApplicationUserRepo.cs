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

    public class ApplicationUserRepo : Repository<ApplicationUser>, IApplicationUser
    {
 private AppsContext _Content;
        public ApplicationUserRepo(AppsContext content) : base(content)
        {
            _Content = content;
        }

        public void Update(ApplicationUser obj)
        {
          _Content.Update(obj);
        }
    }
}
