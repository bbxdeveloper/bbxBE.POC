using bbxBE.POC.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    /// <summary>
    /// Query for finding a product by product code.
    /// </summary>
    public class ProductFindQuery
    {
        private readonly IProductQueryRepository _productQueries;

        public ProductFindQuery(IProductQueryRepository productQueries)
        {
            _productQueries = productQueries;
        }

        public Task<ProductListQueryResponse> Execute(ProductListQueryRequest req)
        {
            return _productQueries.Read(req);
        }
    }
}
