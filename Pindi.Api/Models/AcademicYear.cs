using System.Text.Json.Serialization;

namespace Pindi.Api.Models
{
    public class AcademicYear
    {
        public int Id { get; set; }
        public required string Year { get; set; }

        [JsonIgnore]
        public List<Student>? Students { get; set; }

        [JsonIgnore]
        public List<Course>? Courses { get; set; }
    }
}
