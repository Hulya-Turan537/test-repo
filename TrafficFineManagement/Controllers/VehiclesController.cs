using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Data;
using TrafficFineManagement.Models;
using TrafficFineManagement.Validators;

namespace TrafficFineManagement.Controllers;

public class VehiclesController : Controller
{
    private readonly FineDbContext _db;
    private readonly IValidator<Vehicle> _validator;

    public VehiclesController(FineDbContext db, IValidator<Vehicle> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IActionResult> Index()
    {
        var vehicles = await _db.Vehicles
            .AsNoTracking()
            .OrderBy(v => v.Plate)
            .ToListAsync();
        return View(vehicles);
    }

    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _db.Vehicles
            .AsNoTracking()
            .Include(v => v.Fines)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    public IActionResult Create()
    {
        return View(new Vehicle());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        ModelState.Remove(nameof(Vehicle.Fines));
        vehicle.Plate = VehicleValidator.NormalizePlate(vehicle.Plate);
        vehicle.BrandModel = (vehicle.BrandModel ?? string.Empty).Trim();

        if (!await _validator.ValidateToModelStateAsync(vehicle, ModelState))
        {
            return View(vehicle);
        }

        _db.Vehicles.Add(vehicle);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Araç kaydı oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await _db.Vehicles.FindAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest();
        }

        ModelState.Remove(nameof(Vehicle.Fines));
        vehicle.Plate = VehicleValidator.NormalizePlate(vehicle.Plate);
        vehicle.BrandModel = (vehicle.BrandModel ?? string.Empty).Trim();

        if (!await _validator.ValidateToModelStateAsync(vehicle, ModelState))
        {
            return View(vehicle);
        }

        _db.Update(vehicle);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Araç kaydı güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _db.Vehicles
            .AsNoTracking()
            .Include(v => v.Fines)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _db.Vehicles
            .Include(v => v.Fines)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound();
        }

        if (vehicle.Fines.Count > 0)
        {
            TempData["Error"] = "Bu araca bağlı trafik cezası bulunduğu için silinemez.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _db.Vehicles.Remove(vehicle);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Araç kaydı silindi.";
        return RedirectToAction(nameof(Index));
    }
}
