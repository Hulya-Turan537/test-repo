using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Data;
using TrafficFineManagement.Models;

namespace TrafficFineManagement.Services;

public class ApprovalWorkflowService : IApprovalWorkflowService
{
    private readonly FineDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ApprovalWorkflowService(FineDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public bool CanAct(FineStatus status, UserRole role) =>
        RequiredRole(status) == role;

    public async Task ApproveAsync(int fineId, CancellationToken cancellationToken = default)
    {
        var fine = await LoadAsync(fineId, cancellationToken);
        var user = _currentUser.GetCurrentUser();
        EnsureCanAct(fine, user.Role);

        var previous = fine.Status;
        var next = NextStatusAfterApproval(previous);
        fine.Status = next;
        fine.History.Add(CreateHistory(user.DisplayName, ApprovalActionType.Approved, previous, next, "Onay verildi."));
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int fineId, string reason, CancellationToken cancellationToken = default)
    {
        var trimmed = (reason ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length < 3)
        {
            throw new WorkflowException("Ret nedeni en az 3 karakter olmalıdır.");
        }

        var fine = await LoadAsync(fineId, cancellationToken);
        var user = _currentUser.GetCurrentUser();
        EnsureCanAct(fine, user.Role);

        var previous = fine.Status;
        fine.Status = FineStatus.Reddedildi;
        fine.History.Add(CreateHistory(user.DisplayName, ApprovalActionType.Rejected, previous, FineStatus.Reddedildi, trimmed));
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<TrafficFine> LoadAsync(int fineId, CancellationToken cancellationToken)
    {
        var fine = await _db.TrafficFines
            .Include(f => f.History)
            .FirstOrDefaultAsync(f => f.Id == fineId, cancellationToken);
        if (fine is null)
        {
            throw new WorkflowException("Trafik cezası bulunamadı.");
        }

        return fine;
    }

    private static void EnsureCanAct(TrafficFine fine, UserRole role)
    {
        if (fine.IsClosed)
        {
            throw new WorkflowException("Tamamlanan veya reddedilen cezalar üzerinde işlem yapılamaz.");
        }

        var required = RequiredRole(fine.Status);
        if (required is null || required != role)
        {
            throw new WorkflowException("Bu aşamada işlem yetkiniz yok.");
        }
    }

    // Yeni and Yönetici Onayı wait on the manager; Finans Onayı waits on finance.
    private static UserRole? RequiredRole(FineStatus status) => status switch
    {
        FineStatus.Yeni or FineStatus.YoneticiOnayi => UserRole.Yonetici,
        FineStatus.FinansOnayi => UserRole.Finans,
        _ => null
    };

    private static FineStatus NextStatusAfterApproval(FineStatus current) => current switch
    {
        FineStatus.Yeni => FineStatus.FinansOnayi,
        FineStatus.YoneticiOnayi => FineStatus.FinansOnayi,
        FineStatus.FinansOnayi => FineStatus.Tamamlandi,
        _ => throw new WorkflowException("Bu durumdan onay ile ilerlenemez.")
    };

    private static ApprovalHistory CreateHistory(
        string performedBy,
        ApprovalActionType actionType,
        FineStatus previous,
        FineStatus next,
        string description) =>
        new()
        {
            PerformedBy = performedBy,
            PerformedAt = DateTime.Now,
            ActionType = actionType,
            Description = description,
            PreviousStatus = previous,
            NewStatus = next
        };
}
