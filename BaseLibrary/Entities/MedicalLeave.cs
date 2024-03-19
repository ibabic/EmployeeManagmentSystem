using System.ComponentModel.DataAnnotations;


namespace BaseLibrary.Entities
{
    public class MedicalLeave : OtherBaseEntity
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string MedicalDiagnose { get; set; } = string.Empty;
        [Required]
        public string MedicalRecomendation { get; set; } = string.Empty;
    }
}
