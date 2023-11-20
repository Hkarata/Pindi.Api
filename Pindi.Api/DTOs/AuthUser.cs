namespace Pindi.Api.DTOs
{
    public record struct AuthUser(String Username, String Password, Boolean Role);
}
