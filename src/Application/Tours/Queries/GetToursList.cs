// src/Application/Tours/Queries/GetToursList.cs
using ToursApp.Application.Tours.DTOs;
using ToursApp.Domain.Interfaces;
using ToursApp.Application.Mappers;
using MediatR;                   // For IRequest and IRequestHandler

namespace ToursApp.Application.Tours.Queries;

public record GetToursQuery(bool IncludeInactive = false) 
    : IRequest<IEnumerable<TourDto>>;


public class GetToursQueryHandler : IRequestHandler<GetToursQuery, IEnumerable<TourDto>>
{
    private readonly ITourRepository _repository;

    public GetToursQueryHandler(ITourRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourDto>> Handle(
        GetToursQuery request, 
        CancellationToken cancellationToken)
    {
        var tours = await _repository.GetAllToursAsync(request.IncludeInactive);
        return TourMapper.ToDtos(tours.ToList());
    }
}