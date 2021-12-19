using bbxBE.POC.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    /// <summary>
    /// Query for getting a list of products without applying search criteria - except filtering (in)active products.
    /// </summary>
    public class ProductListQueryForSearch
    {
        private readonly IProductQueryRepository _productQueries;

        public ProductListQueryForSearch(IProductQueryRepository productQueries)
        {
            _productQueries = productQueries;
        }

        public Task<ProductListQueryResponse> Execute(ProductListQueryRequest req)
        {
            return _productQueries.QueryForSearch(req);
        }
    }
}
