namespace Application.DTO
{
    public record CreateUserDto(string Name, string Email, string Password, string ConfirmPassword, string Role, DateOnly DateOfBirth);

    public record ResponseUserDto(string Id, string Name, string Email);

    public record LoginUserDto(string Mail, string Password);

    public record LoginUserResponseDto (bool Succes, string Message);
}
