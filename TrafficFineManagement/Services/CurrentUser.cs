using TrafficFineManagement.Models;

namespace TrafficFineManagement.Services;

public sealed class CurrentUser
{
    public UserRole Role { get; init; }
    public string DisplayName { get; init; } = string.Empty;
}
