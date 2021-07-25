using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BadDriver.RestApi.Jwt
{
    public interface IJwtManager
    {
        IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }
        JwtResult GenerateTokens(string username, IEnumerable<Claim> claims, DateTime utcNow);
        JwtResult Refresh(string refreshToken, string accessToken, DateTime utcNow);
        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshTokenByUsername(string username);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);

    }
}