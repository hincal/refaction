using System;
using System.Collections.Generic;
using Dapper;
using refactor_me.Database;
using refactor_me.Models;
using System.Linq;

namespace refactor_me.Repositories
{
    /// <summary>
    /// Data Access Layer to Product Table
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private const string SQL_SELECT_ALL = "select * from Product";
        private const string SQL_SELECT_BY_NAME = "select * from Product where lower(Name) like @Name";
        private const string SQL_SELECT_BY_ID = "select * from Product where Id = @Id";

        private const string SQL_INSERT = "insert into Product(Id, Name, Description, Price, DeliveryPrice) values (@Id, @Name, @Description, @Price, @DeliveryPrice)";
        private const string SQL_UPDATE = "update Product set Name = @Name, Description = @Description, Price = @Price, DeliveryPrice = @DeliveryPrice where Id = @Id";
        private const string SQL_DELETE = "delete from Product where Id = @Id";

        IDbConnectionFactory _dbConnectionFactory;
        IProductOptionRepository _productOptionRepository;

        public ProductRepository(IDbConnectionFactory dbConnectionFactory, IProductOptionRepository productOptionRepository)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _productOptionRepository = productOptionRepository;
        }

        /// <summary>
        /// Queries entire Product Table
        /// </summary>
        /// <returns>Product entities as List<Product></returns>
        public IList<Product> GetAll()
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var products = dbConn.Query<Product>(SQL_SELECT_ALL).ToList();
                return products;
            }
        }

        /// <summary>
        /// Queries Product Table for the specific Id
        /// </summary>
        /// <param name="id">ProductId</param>
        /// <returns>Matching Product entity</returns>
        public Product GetById(Guid id)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var product = dbConn.Query<Product>(SQL_SELECT_BY_ID, new { Id = id }).ToList().FirstOrDefault();
                return product;
            }
        }

        /// <summary>
        /// Queries Product Table for a specific Name
        /// </summary>
        /// <param name="name">Product Name</param>
        /// <returns>Matching Product entities as List<Product></returns>
        public IList<Product> GetByName(string name)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var products = (List<Product>)dbConn.Query<Product>(SQL_SELECT_BY_NAME, new { Name = name.ToLower() });
                return products;
            }
        }

        /// <summary>
        /// Inserts product entity to Product table
        /// </summary>
        /// <param name="product">product entity</param>
        /// <returns>bool value regarding to outcome</returns>
        public bool Create(Product product)
        {
            var retVal = false;

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var command = new CommandDefinition(SQL_INSERT, product);
                retVal = dbConn.Execute(command) > 0;
            }

            return retVal;
        }

        /// <summary>
        /// Updates product entity
        /// </summary>
        /// <param name="product">product entity</param>
        /// <returns>bool value regarding to outcome</returns>
        public bool Update(Product product)
        {
            var retVal = false;

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var command = new CommandDefinition(SQL_UPDATE, product);
                retVal = dbConn.Execute(command) > 0;
            }

            return retVal;
        }

        /// <summary>
        /// Deletes product from Product table by ProductId
        /// </summary>
        /// <param name="id">ProductId</param>
        public void Delete(Guid id)
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                using (var tran = dbConn.BeginTransaction())
                {
                    try
                    {
                        _productOptionRepository.DeleteByProductId(id);
                        dbConn.Execute(SQL_DELETE, new { Id = id }, tran);

                        tran.Commit();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
