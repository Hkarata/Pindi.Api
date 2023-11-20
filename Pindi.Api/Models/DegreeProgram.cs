using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pindi.Api.Models
{
    public class DegreeProgram
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [JsonIgnore]
        public List<Student>? Students { get; set; }

        [JsonIgnore]
        public List<Course>? Courses { get; set; }
    }
}
