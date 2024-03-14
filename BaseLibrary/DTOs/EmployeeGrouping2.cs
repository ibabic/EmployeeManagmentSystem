using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class EmployeeGrouping2
    {
        [Required]
        public string JobName { get; set; } = string.Empty;
        [Required, Range(1, 99999, ErrorMessage = "You have to select branch")]
        public int BranchId { get; set; }
        [Required, Range(1, 99999, ErrorMessage = "You have to select city")]
        public int CityId { get; set; }
        public string? Other { get; set; }
    }
}
