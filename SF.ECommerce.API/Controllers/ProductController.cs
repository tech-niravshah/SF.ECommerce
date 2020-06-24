using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using SF.ECommerce.API.Model;
using SF.ECommerce.Product.Model;

namespace SF.ECommerce.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ProductController> _logger;
        private readonly IProductCatalogService _service;

        public ProductController(ILogger<ProductController> logger)
        {
            #region
            // SubModule Testing
            #endregion
            _logger = logger;

            var proxyFactory = new ServiceProxyFactory(
                c => new FabricTransportServiceRemotingClientFactory());

            _service = proxyFactory.CreateServiceProxy<IProductCatalogService>
                (new Uri("fabric:/SF.ECommerce/SF.ECommerce.Products")
                ,new ServicePartitionKey(0));
        }
        
        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> GetAsync()
        {
            IEnumerable<Product.Model.Product> allProducts = await _service.GetAllProductsAsync();

            return allProducts.Select(p => new ApiProduct
            {
                Id = p.Id,
                Availability = p.Availability,
                Description = p.Description,
                Name = p.Name,
                Price = p.Price
            });

        }

        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct apiProduct)
        {
            var newProduct = new Product.Model.Product()
            {
                Price = apiProduct.Price,
                Name = apiProduct.Name,
                Description = apiProduct.Description,
                Availability = apiProduct.Availability,
                Id = Guid.NewGuid()
            };

            await _service.AddProductAsync(newProduct);

        }

    }
}
