using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace refactor_me.Repositories
{
    public interface IProductOptionRepository
    {
        IList<ProductOption> GetByProductId(Guid productId);
        ProductOption GetById(Guid id);
        bool Create(ProductOption productOption);
        bool Update(ProductOption productOption);
        void Delete(Guid id);
        void DeleteByProductId(Guid productId);
    }
}
