using BaseLibrary.Entities;

namespace BaseLibrary.DTOs
{
    public class MedicalLeaveDTO : MedicalLeave
    {
        public string? EmployeeFullName { get; set; }
    }
}
