using System.ComponentModel.DataAnnotations;

namespace ShiftSwap.R.PL.Dtos
{
    public class EditProjectDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters.")]
        public string Name { get; set; }
    }
}
