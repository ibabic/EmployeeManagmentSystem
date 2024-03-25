using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string? Role { get; set; }

        [MinLength(5)]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, one special character and be at least 8 characters long")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
