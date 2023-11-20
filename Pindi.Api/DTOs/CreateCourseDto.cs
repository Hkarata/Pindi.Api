using Pindi.Api.Models;

namespace Pindi.Api.DTOs
{
    public record struct CreateCourseDto(String Code, String Name, String Description, String LecturerName, List<DegreeProgram> Programs ,List<AcademicYear> Years);
}
