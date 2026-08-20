using TrafficFineManagement.Models;
using TrafficFineManagement.Services;

namespace TrafficFineManagement.ViewModels;

public class DashboardViewModel
{
    public int VehicleCount { get; set; }
    public int FineCount { get; set; }
    public IReadOnlyDictionary<FineStatus, int> StatusCounts { get; set; } = new Dictionary<FineStatus, int>();
    public IReadOnlyList<TrafficFine> Pending { get; set; } = Array.Empty<TrafficFine>();
    public IReadOnlyList<TrafficFine> Recent { get; set; } = Array.Empty<TrafficFine>();
    public CurrentUser CurrentUser { get; set; } = null!;
}
