using bbxBE.POC.Application.Exceptions;
using bbxBE.POC.Application.Interfaces.Repositories;
using bbxBE.POC.Application.Wrappers;
using bbxBE.POC.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Features.Positions.Queries.GetPositionById
{
    public class GetPositionByIdQuery : IRequest<Response<Position>>
    {
        public Guid Id { get; set; }

        public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, Response<Position>>
        {
            private readonly IPositionRepositoryAsync _positionRepository;

            public GetPositionByIdQueryHandler(IPositionRepositoryAsync positionRepository)
            {
                _positionRepository = positionRepository;
            }

            public async Task<Response<Position>> Handle(GetPositionByIdQuery query, CancellationToken cancellationToken)
            {
                var position = await _positionRepository.GetByIdAsync(query.Id);
                if (position == null) throw new ApiException($"Position Not Found.");
                return new Response<Position>(position);
            }
        }
    }
}