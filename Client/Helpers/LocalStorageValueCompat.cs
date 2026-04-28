namespace Client.Helpers;

internal static class LocalStorageValueCompat
{
    public static string? FromBrowser(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw[0] != '"' || raw[^1] != '"')
        {
            return raw;
        }

        try
        {
            return JsonSerializer.Deserialize<string>(raw);
        }
        catch (JsonException)
        {
            return raw;
        }
    }
}
