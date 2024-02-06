using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IRepository<T>
    {
        List<T> List();
        List<T> ListByFilter(Expression<Func<T, bool>> filter);
        T Get(Expression<Func<T, bool>> filter);
        void Insert(T item);
        void Update(T item);
        void Delete(T item);
    }
}
