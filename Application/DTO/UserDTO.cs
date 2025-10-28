namespace Application.DTO
{
    public record CreateUserDto(string Name, string Email, string Password, string ConfirmPassword);

    public record ResponseUserDto(string Id, string Name, string Email);

    public record LoginUserDto(string Email, string Password);

    public record LoginUserResponseDto (bool Succes, string Message);
}
