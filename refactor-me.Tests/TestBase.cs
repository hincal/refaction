using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using refactor_me.Controllers;
using refactor_me.Database;
using refactor_me.Repositories;
using System;
using System.Configuration;

namespace refactor_me.Tests
{
    public abstract class TestBase
    {
        private const string SQL_INSERT_PRODUCT = "insert into Product(Id, Name, Description, Price, DeliveryPrice) values (@Id, @Name, @Description, @Price, @DeliveryPrice)";
        private const string SQL_INSERT_PRODUCTOPTION = "insert into ProductOption(Id, ProductId, Name, Description) values (@Id, @ProductId, @Name, @Description)";

        private const string SQL_DELETE_PRODUCT = "delete from Product";
        private const string SQL_DELETE_PRODUCTOPTION = "delete from ProductOption";

        internal DbConnectionFactory _dbConnectionFactory;
        internal ProductsController _productController;
        internal ProductOptionsController _productOptionController;

        [TestInitialize]
        public void TestInitialize()
        {
            //  Define data directory and initialize database connection factory
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database"));
            var connectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            _dbConnectionFactory = new DbConnectionFactory(connectionString);

            //  initialize repositories
            var productOptionRepository = new ProductOptionRepository(_dbConnectionFactory);
            var productRepository = new ProductRepository(_dbConnectionFactory, productOptionRepository);

            //  initialize controllers
            _productController = new ProductsController(productRepository);
            _productOptionController = new ProductOptionsController(productOptionRepository);
        }

        private void InitializeDatabase()
        {
            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                //  Truncate Product Table and insert test data
                dbConn.Execute(SQL_DELETE_PRODUCT);
                dbConn.Execute(SQL_INSERT_PRODUCT,
                    new[]
                    {
                        new {Id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"), Name = "Samsung Galaxy S7", Description = "Newest mobile product from Samsung.", Price = 1024.99, DeliveryPrice = 16.99 },
                        new {Id = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"), Name = "Apple iPhone 6S", Description = "Newest mobile product from Apple.", Price = 1299.99, DeliveryPrice = 15.99 }
                    });

                //  Truncate ProductOption table and insert data
                dbConn.Execute(SQL_DELETE_PRODUCTOPTION);
                dbConn.Execute(SQL_INSERT_PRODUCTOPTION,
                    new[]
                    {
                        new {Id = new Guid("0643ccf0-ab00-4862-b3c5-40e2731abcc9"), ProductId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"), Name = "White", Description = "White Samsung Galaxy S7" },
                        new {Id = new Guid("a21d5777-a655-4020-b431-624bb331e9a2"), ProductId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"), Name = "Black", Description = "Black Samsung Galaxy S7" },

                        new {Id = new Guid("5c2996ab-54ad-4999-92d2-89245682d534"), ProductId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"), Name = "Rose Gold", Description = "Gold Apple iPhone 6S" },
                        new {Id = new Guid("9ae6f477-a010-4ec9-b6a8-92a85d6c5f03"), ProductId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"), Name = "White", Description = "White Apple iPhone 6S" },
                        new {Id = new Guid("4e2bc5f2-699a-4c42-802e-ce4b4d2ac0ef"), ProductId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"), Name = "Black", Description = "Black Apple iPhone 6S" }
                    });
            }
        }
    }
}
