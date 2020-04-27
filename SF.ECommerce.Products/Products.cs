using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SF.ECommerce.Product.Model;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SF.ECommerce.Products
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Products : StatefulService, IProductCatalogService
    {
        private IProductRepository _repo;
        public Products(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProductAsync(Product.Model.Product product)
        {
            await _repo.AddProduct(product);
        }

        public async Task<Product.Model.Product[]> GetAllProductsAsync()
        {
            return (await _repo.GetAllProducts()).ToArray();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context =>
            new FabricTransportServiceRemotingListener(context, this))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repo = new ServiceFabricProductRepository(this.StateManager);

            Product.Model.Product product1 = new Product.Model.Product()
            {
                Availability = 5,
                Description = "IPhone",
                Id = Guid.NewGuid(),
                Name = "Iphone 5",
                Price = 200.500
            };

            await _repo.AddProduct(product1);


            await _repo.GetAllProducts();

        }
    }
}
