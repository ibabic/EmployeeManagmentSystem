using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Vacation : OtherBaseEntity
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int NumberOfDays
        {
            get
            {
                int workingDays = 0;
                DateTime date = StartDate;
                while (date <= EndDate)
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workingDays++;
                    }
                    date = date.AddDays(1); 
                }
                return workingDays;
            }
        }
        public VacationType? VacationType { get; set; }
        [Required]
        public int VacationTypeId { get; set; }
    }
}
