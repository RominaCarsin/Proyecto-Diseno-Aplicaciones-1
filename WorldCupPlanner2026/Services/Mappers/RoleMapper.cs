using Domain;
using DTOs;

namespace Services.Mappers;

public class RoleMapper
{
    public static Role ToDomain(RoleDto dto)
    {
        return (Role)dto;
    }

    public static RoleDto ToDto(Role domain)
    {
        return (RoleDto)domain;
    }
}