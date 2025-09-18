using LRA.Gateways.Admin.Interfaces;

namespace LRA.Gateways.Admin.Implementations;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => _httpContextAccessor.HttpContext?.Items["UserId"] as Guid?;
    public string? Email => _httpContextAccessor.HttpContext?.Items["Mail"] as string;
    
    public bool IsSuperAdmin => _httpContextAccessor.HttpContext?.Items["IsSuperAdmin"] as bool? ?? false;
}
