using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Data;

namespace BasketBallLiveScore.Server.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Méthode pour enregistrer un utilisateur
        public async Task<User> RegisterAsync(RegisterRequestDTO request)
        {
            // Créer un nouvel utilisateur
            var user = new User
            {
                Email = request.Email
            };

            // Hachage du mot de passe avec PasswordHasher, qui gère le salt automatiquement
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            // Ajouter l'utilisateur dans la base de données
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Méthode pour se connecter
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            // Chercher l'utilisateur avec l'email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                throw new Exception("Utilisateur introuvable");

            // Vérifier si le mot de passe est correct en utilisant PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Mot de passe incorrect");

            // Créer un token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return new LoginResponseDTO { Token = jwtToken };
        }
    }
}
