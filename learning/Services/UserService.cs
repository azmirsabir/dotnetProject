using learning.Entities;
using learning.Exceptions;
using learning.Extensions;
using learning.Model;
using learning.Model.DTOs;
using learning.Model.Response;
using learning.Services.Interfaces;
using learning.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace learning.Services;

public class UserService(ApplicationDbContext _context) : IUserService
{
    public async Task<PaginatedResult<User>> UsersListAsync(QueryParameters request)
    {
        var users = await _context.Users.
            ApplySearchAndSort(request).
            PaginateAsync(request.PerPage, request.Page);
        
        return users;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var user=await _context.Users.FindAsync(id);
        if(user is null) throw new NotFoundException("User not found");
        return user;
    }

    public async Task<User> SaveUserAsync(UserDto request)
    {
        if (await _context.Users.AnyAsync(user => user.Username == request.Username))
        {
            throw new NotFoundException("User not found");
        }
        var user = new User()
        {
            Username = request.Username,
            Name = request.Name,
            CreatedDate = DateTime.UtcNow,
            Role = "User",
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(int userId,UserDto request)
    {
        var existingUser = await _context.Users.FindAsync(userId);
        if (existingUser is null)
            throw new NotFoundException("User not found");
        
        existingUser.Name = request.Name;
        existingUser.Username = request.Username;
        existingUser.Role = request.Role;
        existingUser.PasswordHash = new PasswordHasher<User>().HashPassword(existingUser, request.Password);
        
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new NotFoundException("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}