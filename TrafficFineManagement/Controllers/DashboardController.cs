using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Data;
using TrafficFineManagement.Models;
using TrafficFineManagement.Services;
using TrafficFineManagement.ViewModels;

namespace TrafficFineManagement.Controllers;

public class DashboardController : Controller
{
    private readonly FineDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public DashboardController(FineDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index()
    {
        var user = _currentUser.GetCurrentUser();
        var fines = _db.TrafficFines.AsNoTracking().Include(f => f.Vehicle);

        var statusCounts = await fines
            .GroupBy(f => f.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var counts = Enum.GetValues<FineStatus>().ToDictionary(s => s, _ => 0);
        foreach (var row in statusCounts)
        {
            counts[row.Key] = row.Count;
        }

        var pendingQuery = user.Role == UserRole.Yonetici
            ? fines.Where(f => f.Status == FineStatus.Yeni || f.Status == FineStatus.YoneticiOnayi)
            : fines.Where(f => f.Status == FineStatus.FinansOnayi);

        var model = new DashboardViewModel
        {
            CurrentUser = user,
            VehicleCount = await _db.Vehicles.CountAsync(),
            FineCount = await fines.CountAsync(),
            StatusCounts = counts,
            Pending = await pendingQuery
                .OrderBy(f => f.FineDate)
                .Take(8)
                .ToListAsync(),
            Recent = await fines
                .OrderByDescending(f => f.Id)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }
}
