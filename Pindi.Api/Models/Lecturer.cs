using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pindi.Api.Models
{
    [PrimaryKey("StaffNumber")]
    public class Lecturer
    {
        public required String StaffNumber { get; set; }

        [MaxLength(50)]
        public required String Name { get; set; }
        public List<Course>? Courses { get; set; }
    }
}
