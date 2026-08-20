using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Models;

public enum FineStatus
{
    [Display(Name = "Yeni")]
    Yeni = 1,

    [Display(Name = "Yönetici Onayı")]
    YoneticiOnayi = 2,

    [Display(Name = "Finans Onayı")]
    FinansOnayi = 3,

    [Display(Name = "Tamamlandı")]
    Tamamlandi = 4,

    [Display(Name = "Reddedildi")]
    Reddedildi = 5
}
