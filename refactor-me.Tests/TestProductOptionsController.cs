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
    public class TestProductOptionsController : TestBase
    {
        private const string SQL_SELECT_BY_PRODUCTID = "select * from ProductOption where productId = @ProductId";
        private const string SQL_SELECT_BY_ID = "select * from ProductOption where Id = @Id";

        #region GetByProductId
        [TestMethod]
        public void GetByProductId_ShouldReturnMatchingProductOptions()
        {
            var productId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3");

            var actionResult = _productOptionController.GetByProductId(productId);
            var contentResult = actionResult as OkNegotiatedContentResult<IList<ProductOption>>;

            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IList<ProductOption>>));
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Count == 3);
        }

        [TestMethod]
        public void GetByProductId_ShouldNotReturnAnyProductOptions()
        {
            var productId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec4");

            var actionResult = _productOptionController.GetByProductId(productId);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NotFound.GetType());
        }
        #endregion

        #region GetById
        [TestMethod]
        public void GetById_ShouldReturnMatchingProductOption()
        {
            var productId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3");
            var productOptionId = new Guid("5c2996ab-54ad-4999-92d2-89245682d534");

            var actionResult = _productOptionController.GetById(productId, productOptionId);
            var contentResult = actionResult as OkNegotiatedContentResult<ProductOption>;

            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<ProductOption>));
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.Name, "Rose Gold");
        }

        [TestMethod]
        public void GetById_ShouldNotReturnAnyProductOption()
        {
            var productId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3");
            var productOptionId = new Guid("5c2996ab-54ad-4999-92d2-89245682d535");

            var actionResult = _productOptionController.GetById(productId, productOptionId);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NotFound.GetType());
        }
        #endregion

        #region Create
        [TestMethod]
        public void Create_ShouldInsertProductOptionToDatabase()
        {
            var productOption = new ProductOption
            {
                ProductId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"),
                Name = "Purple",
                Description = "Purple Samsung Galaxy S7"
            };

            var actionResult = _productOptionController.Create(productOption.ProductId, productOption);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NoContent.GetType());

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var productOptions = dbConn.Query<ProductOption>(SQL_SELECT_BY_PRODUCTID, new { ProductId = productOption.ProductId }).ToList();

                Assert.IsNotNull(productOptions);
                Assert.IsTrue(productOptions.Any(q => q.Id == productOption.Id && q.ProductId == productOption.ProductId && q.Name == productOption.Name && q.Description == productOption.Description));
            }
        }
        #endregion

        #region Update
        [TestMethod]
        public void Update_ShouldUpdateProductOption()
        {
            var productId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3");
            var productOptionId = new Guid("5c2996ab-54ad-4999-92d2-89245682d534");

            var actionResult = _productOptionController.GetById(productId, productOptionId);
            var contentResult = actionResult as OkNegotiatedContentResult<ProductOption>;

            var productOption = contentResult.Content;

            productOption.Name = "Purple Rose Gold";
            productOption.Description = "Purple Gold Apple iPhone 6S";

            actionResult = _productOptionController.Update(productOption.Id, productOption);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NoContent.GetType());

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var products = dbConn.Query<ProductOption>(SQL_SELECT_BY_ID, new { Id = productOption.Id }).ToList();

                Assert.IsNotNull(products);
                Assert.IsTrue(products.Any(q => q.Id == productOption.Id && q.ProductId == productOption.ProductId && q.Name == productOption.Name && q.Description == productOption.Description));
            }
        }
        #endregion

        #region Delete
        [TestMethod]
        public void Delete_ShouldDeleteProductOption()
        {
            var productOptionId = new Guid("5c2996ab-54ad-4999-92d2-89245682d534");

            var actionResult = _productOptionController.Delete(productOptionId);
            Assert.IsInstanceOfType(((StatusCodeResult)actionResult).StatusCode, HttpStatusCode.NoContent.GetType());

            using (var dbConn = _dbConnectionFactory.GetOpenConnection())
            {
                var productOptions = dbConn.Query<ProductOption>(SQL_SELECT_BY_ID, new { Id = productOptionId }).ToList();

                Assert.IsNotNull(productOptions);
                Assert.IsTrue(!productOptions.Any());
            }
        }
        #endregion
    }
}
