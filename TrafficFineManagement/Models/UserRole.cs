using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Models;

public enum UserRole
{
    [Display(Name = "Yönetici")]
    Yonetici = 1,

    [Display(Name = "Finans")]
    Finans = 2
}
