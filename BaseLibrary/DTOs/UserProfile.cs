using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTOs
{
    public class UserProfile : Register
    {
        public int Id { get; set; }
        public string? Role { get; set; }
    }
}
