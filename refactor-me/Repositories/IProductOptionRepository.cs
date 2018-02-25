using System;
using System.Collections.Generic;
using refactor_me.Models;

namespace refactor_me.Repositories
{
    /// <summary>
    /// This interface provides necessary methods for ProductOption Repository.
    /// </summary>
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
