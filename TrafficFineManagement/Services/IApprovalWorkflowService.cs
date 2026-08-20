using TrafficFineManagement.Models;

namespace TrafficFineManagement.Services;

public interface IApprovalWorkflowService
{
    bool CanAct(FineStatus status, UserRole role);
    Task ApproveAsync(int fineId, CancellationToken cancellationToken = default);
    Task RejectAsync(int fineId, string reason, CancellationToken cancellationToken = default);
}
