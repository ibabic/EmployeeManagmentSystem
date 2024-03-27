using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Overtime : OtherBaseEntity
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int numberOfDays => (EndDate - StartDate).Days + 1;
        public OvertimeType? OvertimeType { get; set; }
        [Required]
        public int OvertimeTypeId { get; set; }
    }
}
