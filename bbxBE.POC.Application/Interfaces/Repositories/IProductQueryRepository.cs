using bbxBE.POC.Infrastructure.Persistence.Query;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces.Repositories
{
    public interface IProductQueryRepository
    {
        public Task<ProductListQueryResponse> Query(ProductListQueryRequest req);

        public Task<ProductListQueryResponse> SearchProductByCode(ProductListQueryRequest req);

        public Task<ProductListQueryResponse> SearchProductByName(ProductListQueryRequest req);
    }
}
