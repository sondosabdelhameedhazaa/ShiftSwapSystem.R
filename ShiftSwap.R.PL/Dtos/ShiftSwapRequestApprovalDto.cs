namespace ShiftSwap.R.PL.Dtos
{
    public class ShiftSwapRequestApprovalDto
    {
        public int RequestId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
    }
}
