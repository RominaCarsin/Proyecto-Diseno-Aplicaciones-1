using Domain;
using DTOs;

namespace Services.Mappers;

public static class UserMapper
{
    public static User ToEntity(UserRequest request, IPasswordHasher passwordHasher)
    {
        return new User(
            passwordHasher,
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
            Id = user.UserId,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Roles = RoleMapper.ToDto(user.Roles)
        };
    }
}