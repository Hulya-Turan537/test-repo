using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Data;
using TrafficFineManagement.Models;

namespace TrafficFineManagement.Validators;

public class TrafficFineValidator : AbstractValidator<TrafficFine>
{
    public TrafficFineValidator(FineDbContext db)
    {
        RuleFor(f => f.VehicleId)
            .GreaterThan(0).WithMessage("Araç seçimi zorunludur.")
            .MustAsync(async (vehicleId, cancellation) =>
                await db.Vehicles.AnyAsync(v => v.Id == vehicleId, cancellation))
            .WithMessage("Seçilen araç bulunamadı.");

        RuleFor(f => f.Amount)
            .GreaterThan(0).WithMessage("Ceza tutarı 0'dan büyük olmalıdır.");

        RuleFor(f => f.FineDate)
            .NotEmpty().WithMessage("Ceza tarihi zorunludur.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Ceza tarihi gelecekte olamaz.");

        RuleFor(f => f.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
    }
}
