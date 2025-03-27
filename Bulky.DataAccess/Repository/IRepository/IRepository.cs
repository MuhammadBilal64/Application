using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T will be category or any generic 
        IEnumerable<T> GetAll(string? includeproperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeproperties = null);
        void Add(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
