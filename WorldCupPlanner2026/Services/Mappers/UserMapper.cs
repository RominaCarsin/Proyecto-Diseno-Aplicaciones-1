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
            request.Password
        );
    }

    public static UserResponse FromEntity(User user)
    {
        return new UserResponse
        {
            Name = user.Name,
            Email = user.Email
        };
    }
}