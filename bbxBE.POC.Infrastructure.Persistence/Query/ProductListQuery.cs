using bbxBE.POC.Application.DTOs;
using bbxBE.POC.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    public class ProductListQuery
    {
        private readonly IProductQueryRepository _productQueries;

        public ProductListQuery(IProductQueryRepository productQueries)
        {
            _productQueries = productQueries;
        }

        public Task<IEnumerable<ProductDto>> Execute(string searchString)
        {
            return _productQueries.Query(searchString);
        }
    }
}
