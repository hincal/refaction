using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using refactor_me.Models;
using System.Web.Http.Results;
using Dapper;
using System.Net;
using System.Linq;

namespace refactor_me.Tests
{
    [TestClass]
    public class TestProductsController : TestBase
    {
        private const string SQL_SELECT_BY_NAME = "select * from Product where lower(Name) like @Name";
        private const string SQL_SELECT_BY_ID = "select * from Product where Id = @Id";

        #region GetAll
        [TestMethod]
        public void GetAll_ShouldReturnAllProducts()
        {
            var actionResult = _productController.GetAll();
            var contentResult = actionResult as OkNegotiatedContentResult<IList<Product>>;

            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IList<Product>>));
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.Count, 2);
        }
        #endregion

        #region GetByName
        [TestMethod]
        public void GetByName_ShouldReturnMatchingProducts()
        {
            var actionResult = _productController.GetByName("Apple iPhone 6S");
            var contentResult = actionResult as OkNegotiatedContentResult<IList<Product>>;

            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IList<Product>>));
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Count > 0);
            Assert.AreEqual(contentResult.Content[0].Id, new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"));
        }

        [TestMethod]
        public void GetByName_ShouldNotReturnAnyProducts()
        {
            var actionResult = _productController.GetByName("Xiaomi Mi A1");
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NotFound.GetType());
        }
        #endregion

        #region GetById
        [TestMethod]
        public void GetById_ShouldReturnMatchingProduct()
        {
            var productId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");
            var actionResult = _productController.GetById(productId);
            var contentResult = actionResult as OkNegotiatedContentResult<Product>;

            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<Product>));
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.Name, "Samsung Galaxy S7");
        }

        [TestMethod]
        public void GetById_ShouldNotReturnAnyProduct()
        {
            var productId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a4");
            var actionResult = _productController.GetById(productId);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NotFound.GetType());
        }
        #endregion

        #region Create
        [TestMethod]
        public void Create_ShouldInsertProductToDatabase()
        {
            var product = new Product
            {
                Name = "Xiaomi Mi A1",
                Description = "Newest mobile product from Xiaomi.",
                Price = (decimal)360,
                DeliveryPrice = (decimal)14.99
            };

            var actionResult = _productController.Create(product);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NoContent.GetType());

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var products = dbConn.Query<Product>(SQL_SELECT_BY_NAME, new { Name = product.Name.ToLower() }).ToList();

                Assert.IsNotNull(products);
                Assert.IsTrue(products.Any(q => q.Id == product.Id && q.Name == product.Name && q.Description == product.Description && q.Price == product.Price && q.DeliveryPrice == product.DeliveryPrice));
            }
        }
        #endregion

        #region Update
        [TestMethod]
        public void Update_ShouldUpdateProduct()
        {
            var actionResult = _productController.GetById(new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"));
            var contentResult = actionResult as OkNegotiatedContentResult<Product>;

            var product = contentResult.Content;

            product.Name = "Apple iPhone 7S";
            product.Description = "Mobile phone from Apple.";
            product.Price = (decimal)1249.99;
            product.DeliveryPrice = (decimal)14.99;

            actionResult = _productController.Update(product.Id, product);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NoContent.GetType());

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var products = dbConn.Query<Product>(SQL_SELECT_BY_ID, new { Id = product.Id }).ToList();

                Assert.IsNotNull(products);
                Assert.IsTrue(products.Any(q => q.Id == product.Id && q.Name == product.Name && q.Description == product.Description && q.Price == product.Price && q.DeliveryPrice == product.DeliveryPrice));
            }
        }
        #endregion

        #region Delete
        [TestMethod]
        public void Delete_ShouldDeleteProduct()
        {
            var productId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3");
            var actionResult = _productController.Delete(productId);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NoContent.GetType());

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var products = dbConn.Query<Product>(SQL_SELECT_BY_ID, new { Id = productId }).ToList();

                Assert.IsNotNull(products);
                Assert.IsTrue(!products.Any());
            }
        }
        #endregion
    }
}
