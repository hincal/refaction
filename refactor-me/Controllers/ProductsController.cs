using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using refactor_me.Models;
using refactor_me.Repositories;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            var products = _productRepository.GetAll();
            if (products == null || !products.Any())
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(products);
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetByName(string name)
        {
            var products = _productRepository.GetByName(name);
            if (products == null || !products.Any())
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(products);
        }
        
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetById(Guid id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(product);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult Create(Product product)
        {
            _productRepository.Create(product);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult Update(Guid id, Product product)
        {
            var org = _productRepository.GetById(id);
            if (org == null)
                return StatusCode(HttpStatusCode.NotFound);

            org.Name = product.Name;
            org.Description = product.Description;
            org.Price = product.Price;
            org.DeliveryPrice = product.DeliveryPrice;

            if (_productRepository.Update(org))
                return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.InternalServerError);
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            _productRepository.Delete(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
