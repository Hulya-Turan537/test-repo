using TrafficFineManagement.Models;

namespace TrafficFineManagement.ViewModels;

public class TrafficFineListViewModel
{
    public string? Plate { get; set; }
    public FineStatus? Status { get; set; }
    public IReadOnlyList<TrafficFine> Items { get; set; } = Array.Empty<TrafficFine>();
}
