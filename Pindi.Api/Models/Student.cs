using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pindi.Api.Models
{
    [PrimaryKey("RegistrationNumber")]
    public class Student
    {
        public required String RegistrationNumber { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; }
        public required DegreeProgram Program { get; set; }
        public required AcademicYear Year { get; set; }
        public List<Course>? EnrolledCourses { get; set; }
    }
}
