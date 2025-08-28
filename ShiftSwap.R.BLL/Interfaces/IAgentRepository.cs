using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IAgentRepository : IGenericRepository<Agent>
    {
        Task<Agent> GetByHRIDAsync(string hrid);
        Task<Agent> GetByLoginIDAsync(string loginId);
        Task<Agent> GetByNTNameAsync(string ntName);
        Task<IEnumerable<Agent>> GetAgentsByProjectAsync(int projectId);
        Task<IEnumerable<(Agent Agent, ShiftSchedule Schedule)>> GetAvailableAgentsWithShiftsAsync(DateTime date, int excludeAgentId);

    }

}

