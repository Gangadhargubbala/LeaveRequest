using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace LeaveRequest
{
    public class JWTTokenManager
    {
        public static string GenerateToken(string username)
        {
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];
            var secretbase64 = ConfigurationManager.AppSettings["JwtSecret"];
            var secret = Encoding.UTF8.GetBytes(secretbase64);
            string userRole = new JWTTokenManager().GetUserRole(username); // Create an instance to call the non-static method  
            var securityKey = new SymmetricSecurityKey(secret);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                   new Claim(ClaimTypes.Name, username),
                   new Claim(ClaimTypes.Role, userRole) // Use the correct role  
               };
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetUserRole(string username)
        {
            // Ideally: fetch from database using EF or service  
            if (username == "adminuser") return "Admin";
            return "User";

            if (username == "EmployeeUser") return "Employee";
            return "User";
        }
    }
}