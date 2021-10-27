using bbxBE.POC.Domain.Entities;
using System.Collections.Generic;

namespace bbxBE.POC.Application.Interfaces
{
    public interface IMockService
    {
        List<Position> GetPositions(int rowCount);

        List<Employee> GetEmployees(int rowCount);

        List<Position> SeedPositions(int rowCount);
    }
}