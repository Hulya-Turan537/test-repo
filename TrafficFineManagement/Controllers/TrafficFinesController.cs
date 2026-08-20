using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Data;
using TrafficFineManagement.Models;
using TrafficFineManagement.Validators;
using TrafficFineManagement.ViewModels;

namespace TrafficFineManagement.Controllers;

public class TrafficFinesController : Controller
{
    private readonly FineDbContext _db;
    private readonly IValidator<TrafficFine> _validator;

    public TrafficFinesController(FineDbContext db, IValidator<TrafficFine> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IActionResult> Index(string? plate, FineStatus? status)
    {
        var query = _db.TrafficFines
            .AsNoTracking()
            .Include(f => f.Vehicle)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(plate))
        {
            var normalized = VehicleValidator.NormalizePlate(plate);
            query = query.Where(f => f.Vehicle.Plate.Contains(normalized));
        }

        if (status.HasValue)
        {
            query = query.Where(f => f.Status == status.Value);
        }

        var items = await query
            .OrderByDescending(f => f.FineDate)
            .ThenByDescending(f => f.Id)
            .ToListAsync();

        return View(new TrafficFineListViewModel
        {
            Plate = plate,
            Status = status,
            Items = items
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var fine = await _db.TrafficFines
            .AsNoTracking()
            .Include(f => f.Vehicle)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (fine is null)
        {
            return NotFound();
        }

        return View(fine);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateVehiclesAsync();
        return View(new TrafficFine { FineDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrafficFine fine)
    {
        StripNavProperties();
        fine.Description = string.IsNullOrWhiteSpace(fine.Description) ? null : fine.Description.Trim();
        fine.Status = FineStatus.Yeni;

        if (!await _validator.ValidateToModelStateAsync(fine, ModelState))
        {
            await PopulateVehiclesAsync(fine.VehicleId);
            return View(fine);
        }

        fine.History.Add(new ApprovalHistory
        {
            PerformedBy = "Sistem",
            PerformedAt = DateTime.Now,
            ActionType = ApprovalActionType.Created,
            Description = "Trafik cezası kaydı oluşturuldu.",
            PreviousStatus = FineStatus.Yeni,
            NewStatus = FineStatus.Yeni
        });

        _db.TrafficFines.Add(fine);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Trafik cezası kaydı oluşturuldu.";
        return RedirectToAction(nameof(Details), new { id = fine.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var fine = await _db.TrafficFines.FindAsync(id);
        if (fine is null)
        {
            return NotFound();
        }

        if (fine.IsClosed)
        {
            TempData["Error"] = "Tamamlanan veya reddedilen cezalar düzenlenemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await PopulateVehiclesAsync(fine.VehicleId);
        return View(fine);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrafficFine fine)
    {
        if (id != fine.Id)
        {
            return BadRequest();
        }

        var existing = await _db.TrafficFines.FirstOrDefaultAsync(f => f.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.IsClosed)
        {
            TempData["Error"] = "Tamamlanan veya reddedilen cezalar düzenlenemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        StripNavProperties();
        fine.Description = string.IsNullOrWhiteSpace(fine.Description) ? null : fine.Description.Trim();

        if (!await _validator.ValidateToModelStateAsync(fine, ModelState))
        {
            await PopulateVehiclesAsync(fine.VehicleId);
            return View(fine);
        }

        existing.VehicleId = fine.VehicleId;
        existing.Amount = fine.Amount;
        existing.FineDate = fine.FineDate;
        existing.Description = fine.Description;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Trafik cezası güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private void StripNavProperties()
    {
        ModelState.Remove(nameof(TrafficFine.Vehicle));
        ModelState.Remove(nameof(TrafficFine.History));
        ModelState.Remove(nameof(TrafficFine.IsClosed));
    }

    private async Task PopulateVehiclesAsync(int? selectedId = null)
    {
        var vehicles = await _db.Vehicles
            .AsNoTracking()
            .OrderBy(v => v.Plate)
            .Select(v => new { v.Id, Label = v.Plate + " — " + v.BrandModel })
            .ToListAsync();
        ViewBag.Vehicles = new SelectList(vehicles, "Id", "Label", selectedId);
    }
}
