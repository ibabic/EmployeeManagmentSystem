using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUserAccount accountRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await accountRepository.CreateAsync(user);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await accountRepository.SignInAsync(user);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
        {
            if (token == null) return BadRequest("Model is empty");
            var result = await accountRepository.RefreshTokenAsync(token);
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await accountRepository.GetUsers();
            if(users == null) return NotFound();
            return Ok(users);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(ManageUser manageUser)
        {
            var result = await accountRepository.UpdateUser(manageUser);
            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var users = await accountRepository.GetRoles();
            if (users == null) return NotFound();
            return Ok(users);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await accountRepository.DeleteUser(id);
            return Ok(result);
        }
    }
}
