using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ToDo.Core.DTos;
using ToDo.Core.Models;
using ToDo.Infrastructure.Data;

namespace ToDo.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //DI

        private readonly ToDoDbContext _context;

        public UserController(ToDoDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTos dto)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(ModelState); }

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Message = "Email already exists"
                       
                    });
                }

                if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return BadRequest(new { message = "Invalid email format" });

                var us = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };
                _context.Users.Add(us);
                await _context.SaveChangesAsync();
                var res = new UserResponseDTos
                {
                    Id = us.Id,
                    Email = us.Email,
                    Name = us.Name,
                };

                return Ok(new
                {
                    StatusCode = 200,
                    message = "User registered successfully",
                    user = res
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Something went wrong",
                    Error = ex.Message

                });
            }

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTos dto)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(ModelState); }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);

                if (user == null)
                {
                    return Unauthorized(new
                    {
                        StatusCode = StatusCodes.Status203NonAuthoritative,
                        message = "Invalid email or password"
                    });
                }


                bool isPassValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
                if (!isPassValid) { return Unauthorized(new { message = "Invalid email or password" }); }

                var res = new UserResponseDTos
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                };

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Login successful",
                    user = res

                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Something went wrong",
                    Error = ex.Message

                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"User with Id {id} not found"
                    });

                var res = new UserResponseDTos
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                };

                return Ok(new
                {
                    StatusCode =200 ,
                    Message = "Success",
                    Data = res

                });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Something went wrong",
                    Error = ex.Message

                });
            }
        }
    }
}

