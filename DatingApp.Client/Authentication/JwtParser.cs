namespace DatingApp.Client.Authentication;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        List<Claim> claims = new();
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        Dictionary<string, object>? keyValuePairs =
            JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
            ?? new Dictionary<string, object>();

        ExtractRolesFromJwt(claims, keyValuePairs);

        claims.AddRange(keyValuePairs.Select(kvp =>
            new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty)));

        return claims;
    }

    private static void ExtractRolesFromJwt(List<Claim> claims, Dictionary<string, object> keyValuePairs)
    {
        if (!keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles) || roles is null)
        {
            return;
        }

        string rolesText = roles.ToString()?.Trim().TrimStart('[').TrimEnd(']') ?? string.Empty;
        string[] parsedRoles = rolesText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parsedRoles.Length > 1)
        {
            foreach (string parsedRole in parsedRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, parsedRole.Trim('"')));
            }
        }
        else if (parsedRoles.Length == 1)
        {
            claims.Add(new Claim(ClaimTypes.Role, parsedRoles[0].Trim('"')));
        }

        keyValuePairs.Remove(ClaimTypes.Role);
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}
