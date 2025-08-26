using learning.Entities;
using learning.Model;
using learning.Model.DTOs;
using learning.Model.Response;

namespace learning.Services.Interfaces;

public interface IUserService
{
    Task<PaginatedResult<User>> UsersListAsync(QueryParameters request);
    Task<User> GetUserByIdAsync(int id);
    Task<User> SaveUserAsync(UserDto user);
    Task<User> UpdateUserAsync(int userId,UserDto user);
    Task<bool> DeleteUserAsync(int id);
}