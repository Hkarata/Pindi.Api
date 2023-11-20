namespace Pindi.Api.DTOs
{
    public record struct CourseDto(String Code, String Name, String Description, String LecturerName, List<DegreeProgramDto> Programs ,List<YearDto> Years);
}
