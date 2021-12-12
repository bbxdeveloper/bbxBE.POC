using bbxBE.POC.Application.DTOs;
using bbxBE.POC.Application.Interfaces.Repositories;
using bbxBE.POC.Infrastructure.Persistence.Contexts;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Repositories
{
    public class ProductQueryRepository : IProductQueryRepository
    {
        #region SQL

        private const string QUERY_PRODUCTS = "SELECT * FROM Products";

        #endregion SQL

        private readonly DapperContext _context;

        public ProductQueryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> Query(string searchString)
        {
            using (var connection = _context.CreateConnection())
            {
                var products = await connection.QueryAsync<ProductDto>(QUERY_PRODUCTS);
                if (string.IsNullOrWhiteSpace(searchString))
                {
                    return products.ToList();
                }
                else
                {
                    return products.Where(x => x.ProductCode.Contains(searchString)).ToList();
                }
            }
        }
    }
}
