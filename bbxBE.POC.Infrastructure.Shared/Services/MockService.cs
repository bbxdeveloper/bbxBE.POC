using bbxBE.POC.Application.Interfaces;
using bbxBE.POC.Domain.Entities;
using bbxBE.POC.Infrastructure.Shared.Mock;
using System.Collections.Generic;

namespace bbxBE.POC.Infrastructure.Shared.Services
{
    public class MockService : IMockService
    {
        public List<Position> GetPositions(int rowCount)
        {
            var positionFaker = new PositionInsertBogusConfig();
            return positionFaker.Generate(rowCount);
        }

        public List<Employee> GetEmployees(int rowCount)
        {
            var positionFaker = new EmployeeBogusConfig();
            return positionFaker.Generate(rowCount);
        }

        public List<Position> SeedPositions(int rowCount)
        {
            var seedPositionFaker = new PositionSeedBogusConfig();
            return seedPositionFaker.Generate(rowCount);
        }
    }
}