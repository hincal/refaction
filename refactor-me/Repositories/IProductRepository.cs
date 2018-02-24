using refactor_me.Models;
using System;
using System.Collections.Generic;

namespace refactor_me.Repositories
{
    public interface IProductRepository
    {
        IList<Product> GetAll();
        Product GetById(Guid id);
        IList<Product> GetByName(string name);
        bool Create(Product product);
        bool Update(Product product);
        void Delete(Guid id);
    }
}
