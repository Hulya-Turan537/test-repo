using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Models;
using TrafficFineManagement.Services;

namespace TrafficFineManagement.Controllers;

public class AccountController : Controller
{
    private readonly ICurrentUserAccessor _currentUser;

    public AccountController(ICurrentUserAccessor currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SwitchRole(UserRole role, string? returnUrl)
    {
        _currentUser.SetRole(role);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
