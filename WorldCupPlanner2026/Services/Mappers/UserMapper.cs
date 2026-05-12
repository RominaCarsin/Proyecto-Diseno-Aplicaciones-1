using Domain;
using DTOs;

namespace Services.Mappers;

public class UserMapper
{
    public static User ToEntity(UserRequest request)
    {
        return new User(
            request.Name,
            request.Surname,
            request.Email,
            request.DateOfBirth,
            request.Password,
            RoleMapper.ToDomain(request.Roles)
        );
    }

    public static UserResponse FromEntity(User user)
    {
        return new UserResponse
        {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Roles = RoleMapper.ToDto(user.Roles)
        };
    }
}