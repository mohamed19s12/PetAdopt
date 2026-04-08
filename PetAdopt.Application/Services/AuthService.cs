using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager, IConfiguration configuration , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }


        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            //Mapping RegisterDto to ApplicationUser
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                IsApproved = false
            };
            //Create user with UserManager
            var result = await _userManager.CreateAsync(user, dto.Password);

            //Check if user creation succeeded
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User registration failed: {errors}");
            }
            //if there is no role ... create it
            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));
            }

            //Assign role to user
            await _userManager.AddToRoleAsync(user, dto.Role);

            //I Will Implement It at bellow
            var token =await GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email
            };

        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            //Check if user exists and password is correct
            if (user == null || !await _userManager.CheckPasswordAsync(user , dto.Password) )
            {
                throw new Exception("Invalid email or password");
            }

            //Check if user is approved
            if (!user.IsApproved)
            {
                throw new Exception($"Your account is not approved yet. Please wait for admin approval. {user.Id}");
            }

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = token
            };

        }

        //Generate JWT Token That i used in both Register and Login
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            //Cliams
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            //Add roles to claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            //Credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Descriptor
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(_configuration["JWT:DurationInMinutes"])
                    ),
                signingCredentials: creds
                );
 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
