using TrafficFineManagement.Models;

namespace TrafficFineManagement.Services;

public interface ICurrentUserAccessor
{
    CurrentUser GetCurrentUser();
    void SetRole(UserRole role);
}
