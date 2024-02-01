using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class PasswordTokenManager : IMainService<PasswordTokens>
    {
        IRepository<PasswordTokens> _passwordtokendal;

        public PasswordTokenManager(IRepository<PasswordTokens> passwordtokendal)
        {
            _passwordtokendal = passwordtokendal;
        }

        public void Add(PasswordTokens item)
        {
            _passwordtokendal.Insert(item);
        }

        public void Delete(PasswordTokens item)
        {
            _passwordtokendal.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public PasswordTokens GetByID(int id)
        {
            return _passwordtokendal.Get(x => x.PasswordTokenID == id);
        }

        public List<PasswordTokens> GetList()
        {
            return _passwordtokendal.List();
        }

        public void Update(PasswordTokens item)
        {
            _passwordtokendal.Update(item);
        }
    }
}
