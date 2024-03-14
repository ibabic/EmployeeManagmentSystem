
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        //Relationship: one to many
        [JsonIgnore]
        public List<Department>? Departments { get; set; }
    }
}
