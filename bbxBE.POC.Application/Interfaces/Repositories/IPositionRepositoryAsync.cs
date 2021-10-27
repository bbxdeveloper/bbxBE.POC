using bbxBE.POC.Application.Features.Positions.Queries.GetPositions;
using bbxBE.POC.Application.Parameters;
using bbxBE.POC.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces.Repositories
{
    public interface IPositionRepositoryAsync : IGenericRepositoryAsync<Position>
    {
        Task<bool> IsUniquePositionNumberAsync(string positionNumber);

        Task<bool> SeedDataAsync(int rowCount);

        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPagedPositionReponseAsync(GetPositionsQuery requestParameters);
    }
}