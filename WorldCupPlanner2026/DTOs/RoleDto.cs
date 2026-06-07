namespace DTOs;

[Flags]
public enum RoleDto
{
    None = 0,
    Admin = 1,
    Editor = 2,
    Journalist = 4
}