using AutoMapper;
using bbxBE.POC.Application.Features.Employees.Queries.GetEmployees;
using bbxBE.POC.Application.Features.Positions.Commands.CreatePosition;
using bbxBE.POC.Application.Features.Positions.Queries.GetPositions;
using bbxBE.POC.Domain.Entities;

namespace bbxBE.POC.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Position, GetPositionsViewModel>().ReverseMap();
            CreateMap<Employee, GetEmployeesViewModel>().ReverseMap();
            CreateMap<CreatePositionCommand, Position>();
        }
    }
}