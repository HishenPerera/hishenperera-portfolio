using System.ComponentModel.DataAnnotations;

namespace hishenperera_portfolio.Models
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        // Stores the full number combined by JS (countryCode + localNumber)
        // e.g. "+94712345678"
        [Required]
        [RegularExpression(@"^\+[1-9]\d{6,14}$",
            ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
