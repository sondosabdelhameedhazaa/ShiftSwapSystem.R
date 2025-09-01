using Microsoft.EntityFrameworkCore;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSwap.R.DAL.Models.Enums;

namespace ShiftSwap.R.BLL.Repositories
{
    public class ShiftSwapRequestRepository : GenericRepository<ShiftSwapRequest>, IShiftSwapRequestRepository
    {
        public ShiftSwapRequestRepository(ShiftSwapDbContext context) : base(context) { }

        public async Task<IEnumerable<ShiftSwapRequest>> GetPendingRequestsAsync()
        {
            return await _context.ShiftSwapRequests
                                 .Where(r => r.Status == SwapStatus.Pending)
                                 .Include(r => r.RequestorAgent)
                                 .Include(r => r.TargetAgent)
                                 .Include(r => r.ApprovedBy) 
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ShiftSwapRequest>> GetByAgentIdAsync(int agentId)
        {
            return await _context.ShiftSwapRequests
                                 .Where(r => r.RequestorAgentId == agentId || r.TargetAgentId == agentId)
                                 .Include(r => r.RequestorAgent)
                                 .Include(r => r.TargetAgent)
                                 .Include(r => r.ApprovedBy) 
                                 .ToListAsync();
        }
        public async Task<IEnumerable<ShiftSwapRequest>> GetByDateAsync(DateTime date)
        {
            return await _context.ShiftSwapRequests
                .Where(r => r.SwapDate.Date == date.Date)
                .ToListAsync();
        }

    }
}
