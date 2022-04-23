using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TestApp.Models;
using TestApp.Models.DTO;

namespace TestApp.Services
{
    public interface ILoginService
    {
        User? LoginUser(UserLoginDTO model);
        object GetToken(User user);
    }


    public class LoginService : ILoginService
    {
        private readonly ApplicationContext _context;

        public LoginService(ApplicationContext context)
        {
            _context = context;
        }

        public User? LoginUser(UserLoginDTO model)
        {
            var encodedPassword = EncodePassword(model.Password);
            var user = _context.Users.Where(user => user.Username == model.Username && user.Password == encodedPassword).FirstOrDefault();
            return user;
        }

        private static ClaimsIdentity GetIdentity(string username, Role role)
        {
            // Claims описывают набор базовых данных для авторизованного пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
            };

            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        public object GetToken(User user)
        {
            var identity = GetIdentity(user.Username, (Role)user.Role);

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(JwtConfigurations.Lifetime)),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                token = encodedJwt
            };

            return response;
        }

        private static string EncodePassword(string password)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            var sb = new System.Text.StringBuilder();
            foreach (byte b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}

