using bbxBE.POC.Infrastructure.Persistence.Query;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces.Repositories
{
    public interface IProductQueryRepository
    {
        public Task<ProductListQueryResponse> QueryForSearch(ProductListQueryRequest req);

        public Task<ProductListQueryResponse> SearchProduct(ProductListQueryRequest req);
    }
}
