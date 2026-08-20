using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Models;

public enum ApprovalActionType
{
    [Display(Name = "Oluşturma")]
    Created = 1,

    [Display(Name = "Onay")]
    Approved = 2,

    [Display(Name = "Ret")]
    Rejected = 3
}
