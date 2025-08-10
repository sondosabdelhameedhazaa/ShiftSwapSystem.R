using System.ComponentModel.DataAnnotations;

namespace ShiftSwap.R.PL.Dtos
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters.")]
        public string Name { get; set; }
    }
}
