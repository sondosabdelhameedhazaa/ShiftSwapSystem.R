using System.ComponentModel.DataAnnotations;

namespace ShiftSwap.R.PL.Dtos
{
    public class ShiftSwapRequestCreateDto
    {
        [Required(ErrorMessage = "Target Agent is required")]
        public int TargetAgentId { get; set; }

        [Required(ErrorMessage = "Swap Date is required")]
        [DataType(DataType.Date)]
        public DateTime SwapDate { get; set; }

        [StringLength(200, ErrorMessage = "Comment must be less than 200 characters")]
        public string Comment { get; set; }
    }
}
