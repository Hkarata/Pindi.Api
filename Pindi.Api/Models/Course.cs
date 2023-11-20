using Microsoft.EntityFrameworkCore;

namespace Pindi.Api.Models
{
    [PrimaryKey("Code")]
    public class Course
    {
        public required String Code { get; set; }
        public required String Name { get; set; }
        public required String EnrollmentKey { get; set; }
        public required String Description { get; set; }
        public required Lecturer Lecturer { get; set; }
        public List<Student>? Students { get; set; }
        public List<DegreeProgram>? DegreeProgram { get; set;}
        public List<AcademicYear>? AcademicYear { get; set;}
    }
}
