namespace ShiftSwap.R.PL.Dtos
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AgentInProjectDto> Agents { get; set; }
      
    }
}
