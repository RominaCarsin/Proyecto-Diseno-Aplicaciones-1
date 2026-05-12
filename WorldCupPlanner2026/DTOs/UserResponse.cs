namespace DTOs;

public class UserResponse
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public RoleDto Roles { get; set; }
}