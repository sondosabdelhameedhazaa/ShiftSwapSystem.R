using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IAgentRepository : IGenericRepository<Agent>
    {
        Task<Agent> GetByHRIDAsync(string hrid);
        Task<Agent> GetByLoginIDAsync(string loginId);
        Task<Agent> GetByNTNameAsync(string ntName);
        Task<IEnumerable<Agent>> GetAgentsByProjectAsync(int projectId);
    }
}
