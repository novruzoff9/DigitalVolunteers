using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public  interface IMainService<T>
    {
        List<T> GetList();

        void Add(T item);

        T GetByID(int id);

        void Delete(T item);
        void DeleteById(int id);

        void Update(T item);
    }
}
