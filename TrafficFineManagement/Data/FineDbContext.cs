using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Models;

namespace TrafficFineManagement.Data;

public class FineDbContext : DbContext
{
    public FineDbContext(DbContextOptions<FineDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<TrafficFine> TrafficFines => Set<TrafficFine>();
    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasIndex(v => v.Plate).IsUnique();
            entity.Property(v => v.Plate).HasMaxLength(15).IsRequired();
            entity.Property(v => v.BrandModel).HasMaxLength(120).IsRequired();
            entity.Property(v => v.VehicleType).HasConversion<int>();
        });

        modelBuilder.Entity<TrafficFine>(entity =>
        {
            entity.Property(f => f.Amount).HasPrecision(18, 2);
            entity.Property(f => f.Description).HasMaxLength(500);
            entity.Property(f => f.Status).HasConversion<int>();
            entity.Ignore(f => f.IsClosed);
            entity.HasOne(f => f.Vehicle)
                .WithMany(v => v.Fines)
                .HasForeignKey(f => f.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApprovalHistory>(entity =>
        {
            entity.Property(h => h.PerformedBy).HasMaxLength(100).IsRequired();
            entity.Property(h => h.Description).HasMaxLength(500);
            entity.Property(h => h.ActionType).HasConversion<int>();
            entity.Property(h => h.PreviousStatus).HasConversion<int>();
            entity.Property(h => h.NewStatus).HasConversion<int>();
            entity.HasOne(h => h.TrafficFine)
                .WithMany(f => f.History)
                .HasForeignKey(h => h.TrafficFineId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
