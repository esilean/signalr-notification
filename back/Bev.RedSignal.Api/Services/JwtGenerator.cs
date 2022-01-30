using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bev.RedSignal.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Bev.RedSignal.Api.Services
{
    public class JwtGenerator : IJwtGenerator
    {
        public string Generate(string username)
        {
            var claims = new List<Claim>
                            {
                                new Claim(JwtRegisteredClaimNames.NameId, username)
                            };

          var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a_super_secret_for_signal_connections_poc"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}