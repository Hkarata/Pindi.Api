namespace Pindi.Api.DTOs
{
    public record struct CreateStudentDto(String RegNo, String Name, String Email, Boolean Role, String Phone, String Program, String Year);
}
