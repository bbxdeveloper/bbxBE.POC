using bbxBE.POC.Application.Features.Employees.Queries.GetEmployees;
using bbxBE.POC.Application.Parameters;
using bbxBE.POC.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces.Repositories
{
    public interface IEmployeeRepositoryAsync : IGenericRepositoryAsync<Employee>
    {
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPagedEmployeeReponseAsync(GetEmployeesQuery requestParameter);
    }
}