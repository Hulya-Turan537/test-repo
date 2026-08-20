using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Display(Name = "Plaka")]
    [MaxLength(15)]
    public string Plate { get; set; } = string.Empty;

    [Display(Name = "Araç tipi")]
    public VehicleType VehicleType { get; set; }

    [Display(Name = "Marka / Model")]
    [MaxLength(120)]
    public string BrandModel { get; set; } = string.Empty;

    public ICollection<TrafficFine> Fines { get; set; } = new List<TrafficFine>();
}
