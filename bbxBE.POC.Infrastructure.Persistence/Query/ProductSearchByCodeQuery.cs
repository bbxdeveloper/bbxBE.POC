using bbxBE.POC.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    /// <summary>
    /// Query for searching products by product code.
    /// </summary>
    public class ProductSearchByCodeQuery
    {
        private readonly IProductQueryRepository _productQueries;

        public ProductSearchByCodeQuery(IProductQueryRepository productQueries)
        {
            _productQueries = productQueries;
        }

        public Task<ProductListQueryResponse> Execute(ProductListQueryRequest req)
        {
            return _productQueries.SearchProductByCode(req);
        }
    }
}
