﻿using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        public string CivilId { get; set; } = string.Empty;
        [Required]
        public string FileNumber { get; set; } = string.Empty;
        [Required]
        public string JobName { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required, DataType(DataType.PhoneNumber)]
        public string TelephoneNumber { get; set; } = string.Empty;
        [Required]
        public string Photo { get; set; } = string.Empty;
        public string? Other { get; set; }

        //relationship : Many to one
 
        public Branch? Branch { get; set; }
        public int BranchId { get; set; }
        public City? City { get; set; }
        public int CityId { get; set;}
    }
}
