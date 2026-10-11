using Inventory.Application.Common.Interfaces;

namespace Inventory.Api.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserName =>
        _httpContextAccessor.HttpContext?.User.FindFirst("preferred_username")?.Value
        ?? _httpContextAccessor.HttpContext?.User.Identity?.Name
        ?? "System";
}