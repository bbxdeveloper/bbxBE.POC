using bbxBE.POC.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    /// <summary>
    /// Query for searching products by product name.
    /// </summary>
    public class ProductSearchByNameQuery
    {
        private readonly IProductQueryRepository _productQueries;

        public ProductSearchByNameQuery(IProductQueryRepository productQueries)
        {
            _productQueries = productQueries;
        }

        public Task<ProductListQueryResponse> Execute(ProductListQueryRequest req)
        {
            return _productQueries.SearchProductByName(req);
        }
    }
}
