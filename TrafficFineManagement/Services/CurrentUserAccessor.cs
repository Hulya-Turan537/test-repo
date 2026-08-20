using TrafficFineManagement.Models;

namespace TrafficFineManagement.Services;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    public const string RoleSessionKey = "CurrentRole";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser GetCurrentUser()
    {
        var role = ReadRole();
        return new CurrentUser
        {
            Role = role,
            DisplayName = role == UserRole.Yonetici
                ? "Ayşe Yılmaz (Yönetici)"
                : "Mehmet Kaya (Finans)"
        };
    }

    public void SetRole(UserRole role)
    {
        var session = GetSession();
        session.SetString(RoleSessionKey, role.ToString());
    }

    private UserRole ReadRole()
    {
        var value = GetSession().GetString(RoleSessionKey);
        return Enum.TryParse<UserRole>(value, out var role) ? role : UserRole.Yonetici;
    }

    private ISession GetSession()
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP bağlamı yok.");
        return context.Session;
    }
}
