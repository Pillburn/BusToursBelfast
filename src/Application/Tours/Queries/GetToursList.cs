// src/Application/Tours/Queries/GetToursList.cs
using ToursApp.Application.Tours.DTOs;
using ToursApp.Domain.Interfaces;
using MediatR;                   // For IRequest and IRequestHandler
using AutoMapper;
namespace ToursApp.Application.Tours.Queries;

public record GetToursQuery(bool IncludeInactive = false) 
    : IRequest<IEnumerable<TourDto>>;


public class GetToursQueryHandler : IRequestHandler<GetToursQuery, IEnumerable<TourDto>>
{
    private readonly ITourRepository _repository;
    private readonly IMapper _mapper;

    public GetToursQueryHandler(ITourRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TourDto>> Handle(
        GetToursQuery request, 
        CancellationToken cancellationToken)
    {
        var tours = await _repository.GetAllToursAsync(request.IncludeInactive);
        return _mapper.Map<IEnumerable<TourDto>>(tours);
    }
}