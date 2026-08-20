using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Models;

public class ApprovalHistory
{
    public int Id { get; set; }

    public int TrafficFineId { get; set; }

    public TrafficFine TrafficFine { get; set; } = null!;

    [Display(Name = "İşlemi yapan")]
    [MaxLength(100)]
    public string PerformedBy { get; set; } = string.Empty;

    [Display(Name = "İşlem tarihi")]
    public DateTime PerformedAt { get; set; }

    [Display(Name = "İşlem tipi")]
    public ApprovalActionType ActionType { get; set; }

    [Display(Name = "Açıklama / ret nedeni")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Önceki durum")]
    public FineStatus PreviousStatus { get; set; }

    [Display(Name = "Yeni durum")]
    public FineStatus NewStatus { get; set; }
}
