using bbxBE.POC.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    public class SumReportQuery
    {
        private readonly IProductQueryRepository _productQueries;

        public SumReportQuery(IProductQueryRepository productQueries)
        {
            _productQueries = productQueries;
        }

        public Task<ReportDataQueryResponse> Execute(ReportDataQueryRequest req)
        {
            return _productQueries.ReadReportData(req);
        }
    }
}
