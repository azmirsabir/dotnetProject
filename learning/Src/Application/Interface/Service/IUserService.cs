using learning.Application.Model.DTOs;
using learning.Application.Model.Response;
using learning.Domain.Entities;

namespace learning.Application.Interface.Service;

public interface IUserService
{
    Task<PaginatedResult<User>> UsersListAsync(QueryParameters request);
    Task<User> GetUserByIdAsync(int id);
    Task<User> SaveUserAsync(UserDto user);
    Task<User> UpdateUserAsync(int userId,UserDto user);
    Task<bool> DeleteUserAsync(int id);
}