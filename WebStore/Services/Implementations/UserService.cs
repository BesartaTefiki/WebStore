using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebStore.Data;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class UserService : IUserService
    {
        private readonly WebStoreContext _context;
        private readonly IConfiguration _configuration;

        public UserService(WebStoreContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Username == request.Username);

            if (exists)
                return null;

            var simpleRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == "simple");

            if (simpleRole == null)
                throw new InvalidOperationException("Default role 'simple' not found in database.");

            var client = new Client
            {
                FullName = request.Username,
                Email = $"{request.Username}@example.com"
            };

            _context.Clients.Add(client);

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                RoleId = simpleRole.Id,
                Client = client
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = simpleRole.Name
            };
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == request.Username &&
                    u.Password == request.Password);

            if (user == null)
                return null;

            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            if (user.ClientId.HasValue)
            {
                claims.Add(new Claim("client_id", user.ClientId.Value.ToString()));
            }

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["ExpireMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                Username = user.Username,
                Role = user.Role.Name,
                ClientId = user.ClientId
            };
        }
    }
}
