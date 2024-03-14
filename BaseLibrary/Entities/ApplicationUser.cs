using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]      
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        public string Password { get; set; }
    }
}
