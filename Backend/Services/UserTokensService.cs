﻿using AutoMapper;
using Backend.DTOs;
using FreelanceLand.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Interfaces.ServiceInterfaces;

namespace Backend.Services
{
    public class UserTokensService : IUserTokensService
    {
        private EFGenericRepository<User> userRepo;
        private EFGenericRepository<UserRoles> rolesRepo;
        private readonly IMapper _mapper;

        public UserTokensService(IMapper mapper, ApplicationContext context)
        {
            userRepo = new EFGenericRepository<User>(context);
            rolesRepo = new EFGenericRepository<UserRoles>(context);
            _mapper = mapper;
        }

        public async Task<string> CreateToken(UserAccountDTO user)
        {
            var username = user.Login;
            var password = user.Password;

            var identity = await GetIdentity(username, password);

            if (identity == null)
            {
                return "";
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                login = identity.Name,
                id = user.Id,
                email = user.Email,
                role = identity.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
            };

            string token = JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return token;
           }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            User person = (await userRepo.GetAsync(u => u.Login == username)).FirstOrDefault();

            string role = (await rolesRepo.GetAsync(r => r.Id == person.UserRoleId)).FirstOrDefault().Type;
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            
            return null;
        }
    }
}
