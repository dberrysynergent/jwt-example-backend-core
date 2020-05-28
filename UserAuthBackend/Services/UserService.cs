using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using UserAuthBackend.Helpers;
using Microsoft.Extensions.Options;
using UserAuthBackend.Interfaces;
using System.Security.Principal;

namespace UserAuthBackend.Services
{
    public class UserService : IUserService
    {
        private List<User> Users = new List<User>
        {
            new User { UserName = "test", Password = "testing", Token = null },
            new User { UserName = "test2", Password = "testing2", Token = null }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string authenticatedUsername, string password)
        {
            var authenticatedUser = Users.SingleOrDefault<User>(x => x.UserName == authenticatedUsername && x.Password == password);
            // return null if authenticatedUser not found
            if (authenticatedUser == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, authenticatedUser.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            authenticatedUser.Token = tokenHandler.WriteToken(token);

            return authenticatedUser;
        }

        public IEnumerable<User> GetAll()
        {
            return Users;
        }

        public bool IsAuthenticated(User user)
        {
            return ValidateToken(user.Token);
        }

        private bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }

        private TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(key) // The same key as the one that generate the token
            };
        }
    }
}
