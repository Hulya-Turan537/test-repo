using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrafficFineManagement.Models;

public class TrafficFine
{
    public int Id { get; set; }

    [Display(Name = "Araç")]
    public int VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    [Display(Name = "Ceza tutarı")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Display(Name = "Ceza tarihi")]
    [DataType(DataType.Date)]
    public DateTime FineDate { get; set; }

    [Display(Name = "Açıklama")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Durum")]
    public FineStatus Status { get; set; } = FineStatus.Yeni;

    public ICollection<ApprovalHistory> History { get; set; } = new List<ApprovalHistory>();

    public bool IsClosed => Status is FineStatus.Tamamlandi or FineStatus.Reddedildi;
}
