using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using SF.ECommerce.Product.Model;
using SF.ECommerce.Products;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace SF.ECommerce.Products
{
    public class ServiceFabricProductRepository : IProductRepository
    {
        private IReliableStateManager _stateManager;
        public ServiceFabricProductRepository(IReliableStateManager reliableStateManager)
        {
            _stateManager = reliableStateManager;
        }
        public async Task AddProduct(Product.Model.Product product)
        {
            IReliableDictionary<Guid, Product.Model.Product> products = await 
                _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product.Model.Product>>("products");

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);
                await tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product.Model.Product>> GetAllProducts()
        {
            IReliableDictionary<Guid, Product.Model.Product> products = await 
                _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product.Model.Product>>("products");

            var result = new List<Product.Model.Product>();

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, Product.Model.Product>> allProducts = await
                    products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, Product.Model.Product>> enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product.Model.Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;

        }
    }
}
