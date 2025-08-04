using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IShiftSwapRequestRepository : IGenericRepository<ShiftSwapRequest>
    {
        Task<IEnumerable<ShiftSwapRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<ShiftSwapRequest>> GetByAgentIdAsync(int agentId);
    }
}
