using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IShiftSwapRequestRepository : IGenericRepository<ShiftSwapRequest>
    {
        Task<IEnumerable<ShiftSwapRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<ShiftSwapRequest>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<ShiftSwapRequest>> GetByDateAsync(DateTime date);

    }
}
