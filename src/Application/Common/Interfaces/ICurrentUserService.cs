using System.Security.Claims;
using Microsoft.AspNetCore.Http;// Application/Common/Interfaces/ICurrentUserService.cs
public interface ICurrentUserService
{
    string? UserId { get; }
}

// Infrastructure/Services/CurrentUserService.cs
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => 
    _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}