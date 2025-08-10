using ShiftSwap.R.DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShiftSwap.R.PL.Dtos
{
    public class EditAgentDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string HRID { get; set; }

        [Required]
        public string LoginID { get; set; }

        [Required]
        public string NTName { get; set; }

        [Required]
        public AgentRole Role { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? TeamLeaderId { get; set; }
    }
}
