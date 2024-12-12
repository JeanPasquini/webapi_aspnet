﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webAPI_ASPNET.Models;
using webAPI_ASPNET.Repositorios.Interfaces;

namespace webAPI_ASPNET.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userRepositorio;
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration, IUser userRepositorio)
        {
            _configuration = configuration;
            _userRepositorio = userRepositorio;
        }
        [Authorize("perm")]
        [HttpGet]
        public async Task<ActionResult<List<UserWithDepartment>>> getAll()
        {
            List<UserWithDepartment> users = await _userRepositorio.getAll();
            return Ok(users);
        }
        [Authorize("perm")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> getById(int id)
        {
            User user = await _userRepositorio.getById(id);
            return Ok(user);
        }
        [Authorize("perm")]
        [HttpGet("Department/{id}")]
        public async Task<ActionResult<UserWithDepartment>> getUserIDepartmentById(int id)
        {
            UserWithDepartment user = await _userRepositorio.getUserIDepartmentById(id);
            return Ok(user);
        }
        [Authorize("perm")]
        [HttpGet("Information/{username}/{password}")]
        public async Task<ActionResult<UserWithDepartment>> getUserIDepartmentByUsernameAndPassword(string username, string password)
        {
            UserWithDepartment user = await _userRepositorio.getUserIDepartmentByUsernameAndPassword(username, password);
            return Ok(user);
        }

        [Authorize("perm")]
        [HttpGet("ButtonRelation/{id}")]
        public async Task<ActionResult<UserWithButton>> getUserIButtonById(int id)
        {
            UserWithButton userButton = await _userRepositorio.getUserIButtonById(id);
            return Ok(userButton);
        }

        [Authorize("perm")]
        [HttpPost]
        public async Task<ActionResult<User>> post([FromBody] User userModel)
        {
            User user = await _userRepositorio.post(userModel);

            return Ok(user);
        }
        [Authorize("perm")]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> put([FromBody] User userModel, int id)
        {
            userModel.ID = id;
            User user = await _userRepositorio.put(userModel, id);

            return Ok(user);
        }
        [Authorize("perm")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> delete(int id)
        { 
            bool apagado = await _userRepositorio.delete(id);
            return Ok(apagado);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            var user = _userRepositorio.Authenticate(login.USERNAME, login.PASSWORD);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.USERNAME),
                    new Claim(ClaimTypes.Role, "ADM")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            }; 

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
