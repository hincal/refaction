using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using refactor_me.Models;
using refactor_me.Repositories;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductOptionsController : ApiController
    {
        private IProductOptionRepository _productOptionRepository;

        public ProductOptionsController(IProductOptionRepository productOptionRepository)
        {
            _productOptionRepository = productOptionRepository;
        }

        [Route("{productId}/options")]
        [HttpGet]
        public IHttpActionResult GetByProductId(Guid productId)
        {
            var productOptions = _productOptionRepository.GetByProductId(productId);
            if (productOptions == null || !productOptions.Any())
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(productOptions);
        }

        [Route("{productId}/options/{id}")]
        [HttpGet]
        public IHttpActionResult GetById(Guid productId, Guid id)
        {
            var productOption = _productOptionRepository.GetById(id);
            if (productOption == null)
                return StatusCode(HttpStatusCode.NotFound);
            return Ok(productOption);
        }

        [Route("{productId}/options")]
        [HttpPost]
        public IHttpActionResult Create(Guid productId, ProductOption option)
        {
            option.ProductId = productId;

            _productOptionRepository.Create(option);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{productId}/options/{id}")]
        [HttpPut]
        public IHttpActionResult Update(Guid id, ProductOption option)
        {
            var org = _productOptionRepository.GetById(id);
            if (org == null)
                return StatusCode(HttpStatusCode.NotFound);

            org.Name = option.Name;
            org.Description = option.Description;

            if (_productOptionRepository.Update(org))
                return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.InternalServerError);
        }

        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            _productOptionRepository.Delete(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
