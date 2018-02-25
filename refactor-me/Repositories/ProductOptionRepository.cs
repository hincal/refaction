using System;
using System.Collections.Generic;
using refactor_me.Models;
using refactor_me.Database;
using Dapper;
using System.Linq;

namespace refactor_me.Repositories
{
    /// <summary>
    /// Data Access Layer to ProductOption Table
    /// </summary>
    public class ProductOptionRepository : IProductOptionRepository
    {
        private const string SQL_SELECT_BY_PRODUCTID = "select * from ProductOption where productId = @ProductId";
        private const string SQL_SELECT_BY_ID = "select * from ProductOption where Id = @Id";

        private const string SQL_INSERT = "insert into ProductOption (Id, ProductId, Name, Description) values (@Id, @ProductId, @Name, @Description)";
        private const string SQL_UPDATE = "update ProductOption set ProductId = @ProductId, Name = @Name, Description = @Description where Id = @Id";
        private const string SQL_DELETE = "delete from ProductOption where Id = @Id";
        private const string SQL_DELETE_BY_PRODUCTID = "delete from ProductOption where ProductId = @ProductId";

        IDbConnectionFactory _dbConnectionFactory;

        public ProductOptionRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Queries ProductOption table by ProductId
        /// </summary>
        /// <param name="productId">ProductId</param>
        /// <returns>ProductOption entities as List<ProductOption></returns>
        public IList<ProductOption> GetByProductId(Guid productId)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var productOptions = dbConn.Query<ProductOption>(SQL_SELECT_BY_PRODUCTID, new { ProductId = productId }).ToList();
                return productOptions;
            }
        }

        /// <summary>
        /// Queries ProductOption table by Id
        /// </summary>
        /// <param name="id">ProductOptionId</param>
        /// <returns>Matching ProductOption entity</returns>
        public ProductOption GetById(Guid id)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var productOption = dbConn.Query<ProductOption>(SQL_SELECT_BY_ID, new { Id = id }).ToList().FirstOrDefault();
                return productOption;
            }
        }

        /// <summary>
        /// Inserts productOption entity to ProductOption table
        /// </summary>
        /// <param name="productOption">productOption entity</param>
        /// <returns>bool value regarding to outcome</returns>
        public bool Create(ProductOption productOption)
        {
            var retVal = false;

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var command = new CommandDefinition(SQL_INSERT, productOption);
                retVal = dbConn.Execute(command) > 0;
            }

            return retVal;
        }

        /// <summary>
        /// Updates productOption entity
        /// </summary>
        /// <param name="productOption">productOption entity</param>
        /// <returns>bool value regarding to outcome</returns>
        public bool Update(ProductOption productOption)
        {
            var retVal = false;

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var command = new CommandDefinition(SQL_UPDATE, productOption);
                retVal = dbConn.Execute(command) > 0;
            }

            return retVal;
        }

        /// <summary>
        /// Deletes productOption from ProductOption table by ProductOptionId
        /// </summary>
        /// <param name="id">ProductOptionId</param>
        public void Delete(Guid id)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                dbConn.Execute(SQL_DELETE, new { Id = id });
            }
        }

        /// <summary>
        /// Deletes productOption entities from ProductOption table by ProductId
        /// </summary>
        /// <param name="productId">productId</param>
        public void DeleteByProductId(Guid productId)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                dbConn.Execute(SQL_DELETE_BY_PRODUCTID, new { ProductId = productId });
            }
        }
    }
}
