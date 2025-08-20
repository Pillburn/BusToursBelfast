using System.Security.Claims;
using Microsoft.AspNetCore.Http;// Application/Common/Interfaces/ICurrentUserService.cs
public interface ICurrentUserService
{
    string? UserId { get; }
}