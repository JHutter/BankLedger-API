using BankLedgerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BankLedgerAPI.Utilities
{
    public static class TokenUtils
    {
        private static readonly byte[] _key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("BLA_JWT_SECRET")); // TODO fix this _appSettings.Secret
        // TODO add request IP to claims
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                                new Claim(ClaimTypes.Name, user.Id.ToString()),
                                new Claim(ClaimTypes.UserData, user.Username),
                                new Claim(ClaimTypes.Role, "user")

                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = "BLA",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // TODO: any way to use this in Startup.cs to avoid repeating token validation params?
        public static TokenValidationParameters GetValidationParameters()
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters() {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateActor = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(_key)
            };

            return validationParameters;
        }

        public static bool ValidateToken(string tokenString, out SecurityToken securityToken)
        {
            try
            {
                
                tokenString = tokenString.Replace("Bearer ", "");
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                IPrincipal principal = handler.ValidateToken(tokenString, GetValidationParameters(), out securityToken);
                return true;
            }
            catch (Exception)
            {
                securityToken = null;
                return false;
            }
        }

        public static bool IsValidFormatTokenString(string tokenString)
        {
            int sectionCount = 3;
            return (!String.IsNullOrWhiteSpace(tokenString) && tokenString.Contains("Bearer ") && tokenString.Split(".").Length == sectionCount);
        }

        public static string GetUsernameFromToken(string tokenString,SecurityToken securityToken)
        {
            return GetClaimValueByName(tokenString, ClaimTypes.UserData);
        }

        public static int GetUserIDFromToken(string tokenString, SecurityToken securityToken)
        {
            if (int.TryParse(GetClaimValueByName(tokenString, ClaimTypes.Name), out int id))
            {
                return id;
            }
            else
            {
                return 0;
            }

        }

        private static string GetClaimValueByName(string tokenString, string claimName)
        {
            tokenString = tokenString.Replace("Bearer ", "");
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(tokenString, GetValidationParameters(), out _)?.FindFirst(claimName)?.Value;
        }

    }
}
