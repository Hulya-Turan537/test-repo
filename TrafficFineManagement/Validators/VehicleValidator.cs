using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Data;
using TrafficFineManagement.Models;

namespace TrafficFineManagement.Validators;

public class VehicleValidator : AbstractValidator<Vehicle>
{
    public VehicleValidator(FineDbContext db)
    {
        RuleFor(v => v.Plate)
            .NotEmpty().WithMessage("Plaka zorunludur.")
            .MaximumLength(15).WithMessage("Plaka en fazla 15 karakter olabilir.")
            .Must(BeValidTurkishPlate).WithMessage("Geçerli bir Türkiye plakası girin. Örnek: 34 ABC 123")
            .MustAsync(async (vehicle, plate, cancellation) =>
            {
                var normalized = NormalizePlate(plate);
                return !await db.Vehicles.AnyAsync(
                    v => v.Plate == normalized && v.Id != vehicle.Id,
                    cancellation);
            }).WithMessage("Bu plaka zaten kayıtlı.");

        RuleFor(v => v.VehicleType)
            .IsInEnum().WithMessage("Geçerli bir araç tipi seçin.");

        RuleFor(v => v.BrandModel)
            .NotEmpty().WithMessage("Marka / model zorunludur.")
            .MaximumLength(120).WithMessage("Marka / model en fazla 120 karakter olabilir.");
    }

    public static string NormalizePlate(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            return string.Empty;
        }

        var compact = string.Join(' ',
            plate.Trim().ToUpperInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return compact;
    }

    private static bool BeValidTurkishPlate(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            return false;
        }

        var normalized = NormalizePlate(plate).Replace(" ", string.Empty);
        return System.Text.RegularExpressions.Regex.IsMatch(
            normalized,
            @"^(0[1-9]|[1-7][0-9]|8[01])[A-Z]{1,3}[0-9]{2,4}$");
    }
}
