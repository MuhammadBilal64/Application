using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.DataAccess.Data;
using BookedIn.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace Application.DataAccess.IRepository
{
    public class Repository<T>: IRepository<T> where T : class
    {
        internal DbSet<T> Dbset;
        private readonly AppsContext _content;
        public Repository(AppsContext content)
        {

            _content = content;
            this.Dbset = _content.Set<T>();
            content.Products.Include(u => u.Category).Include(u=>u.CategoryId);


        }
        public void Add(T entity)
        {
           Dbset.Add(entity);
            
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeproperties = null)
        {
            IQueryable<T> query = Dbset;
            query= query.Where(filter);
            if (!String.IsNullOrEmpty(includeproperties))
            {
                foreach (var property in includeproperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }

            }
            return query.FirstOrDefault();
        }
        //Category,CoverType
        public IEnumerable<T> GetAll(string ? includeproperties=null )
        {
            IQueryable<T> query = Dbset;
            if(!String.IsNullOrEmpty(includeproperties))
            {
                foreach (var property in includeproperties
                    .Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(property);
                }

            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            Dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            Dbset.RemoveRange(entity);
        }

       
    }
}
