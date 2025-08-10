using System.ComponentModel.DataAnnotations;

namespace ShiftSwap.R.PL.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Please select identifier type")]
        public string IdentifierType { get; set; } // NTName, HRID, LoginID

        [Required(ErrorMessage = "Please enter your identifier")]
        public string Identifier { get; set; }
    }
}
