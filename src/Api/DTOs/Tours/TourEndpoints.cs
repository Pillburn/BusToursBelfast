using ToursApp.Domain.Interfaces;
internal static class TourEndpoints
{
    public static void MapTourEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/tour").WithTags("Tour");

        group.MapGet("/", GetAllTours);
        group.MapGet("/{id}", GetTourById);
    }

    private static async Task<IResult> GetAllTours(ITourRepository tourRepository)
    {
        var tours = await tourRepository.GetAllToursAsync();
        return Results.Ok(tours);
    }

    private static async Task<IResult> GetTourById(string id, ITourRepository tourRepository)
    {
        var tour = await tourRepository.GetTourByIdAsync(id);
        return tour is not null ? Results.Ok(tour) : Results.NotFound();
    }
}