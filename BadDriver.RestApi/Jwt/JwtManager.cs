using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace BadDriver.RestApi.Jwt
{
    public class JwtManager : IJwtManager
    {
        private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;

        public JwtManager()
        {
            _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>(StringComparer.OrdinalIgnoreCase);
        }

        public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();

        public JwtResult GenerateTokens(string username, IEnumerable<Claim> claims, DateTime utcNow)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

            var jwtToken = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: shouldAddAudienceClaim ? AuthOptions.Audience : string.Empty,
                claims: claims,
                expires: utcNow.AddMinutes(AuthOptions.AccessTokenLifetime),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new RefreshToken
            {
                Username = username,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = utcNow.AddMinutes(AuthOptions.RefreshTokenLifetime)
            };

            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (s, token) => refreshToken);

            return new JwtResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public JwtResult Refresh(string refreshToken, string accessToken, DateTime utcNow)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken);

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var username = principal.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

            if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (existingRefreshToken.Username != username || existingRefreshToken.ExpireAt < utcNow)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return GenerateTokens(username, principal.Claims.ToArray(), utcNow);
        }

        public void RemoveExpiredRefreshTokens(DateTime now)
        {
            var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
            }
        }

        public void RemoveRefreshTokenByUsername(string username)
        {
            var refreshTokens = _usersRefreshTokens.Where(x => x.Value.Username == username).ToList();
            foreach (var refreshToken in refreshTokens)
            {
                _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
            }
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidAudience = AuthOptions.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                    },
                    out var validatedToken);

            return (principal, validatedToken as JwtSecurityToken);

        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}