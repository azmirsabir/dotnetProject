using learning.Application.Interface.Service;
using learning.Application.Model.DTOs;
using learning.Application.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities_User = learning.Domain.Entities.User;
using User = learning.Domain.Entities.User;

namespace learning.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class UserController(IUserService _userService): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<Entities_User>>> SearchUsers([FromQuery] QueryParameters request)
    {
        var users = await _userService.UsersListAsync(request);
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Response<Entities_User>>> GetUserById([FromRoute] int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(new Response<Entities_User>("User found",user));    
    }
    
    [HttpPost]
    public async Task<ActionResult<Response<Entities_User>>> StoreUser(UserDto request)
    {
        var user = await _userService.SaveUserAsync(request);
        return Ok(new Response<Entities_User>("User saved successfully",user));
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Response<Entities_User>>> UpdateUser([FromRoute] int id,UserDto user)
    {
        var updatedUser=await _userService.UpdateUserAsync(id,user);
        return Ok(new Response<Entities_User>("User updated successfully",updatedUser));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<string>>> DeleteUser([FromRoute] int id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok(new Response<string>($"User Id {id} deleted"));
    }
}