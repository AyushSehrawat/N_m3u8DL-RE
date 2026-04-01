namespace N_m3u8DL_RE.Common.Util;

public static class CookieUtil
{
    /// <summary>
    /// Parse a Netscape-format cookie file and return a cookie header string.
    /// Format: name1=value1; name2=value2; ...
    /// 
    /// Netscape cookie file format (tab-separated):
    /// domain \t httponly \t path \t secure \t expiry \t name \t value
    /// 
    /// Lines starting with '#' are comments. Blank lines are skipped.
    /// Values may contain any characters (JSON, URL-encoded, etc.) since
    /// fields are strictly tab-delimited.
    /// </summary>
    /// <param name="filePath">Path to the Netscape cookie file</param>
    /// <returns>A cookie header string ready for use in HTTP requests</returns>
    /// <exception cref="FileNotFoundException">If the file does not exist</exception>
    /// <exception cref="FormatException">If the file contains no valid cookie lines</exception>
    public static string ParseCookieFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cookie file not found: {filePath}");

        var cookies = new List<string>();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip blank lines and comments
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                continue;

            // Split by tab — Netscape format is strictly tab-delimited
            var fields = trimmed.Split('\t');

            // A valid Netscape cookie line has exactly 7 fields:
            // domain, httponly, path, secure, expiry, name, value
            if (fields.Length < 7)
                continue;

            var name = fields[5].Trim();
            // Value may contain tabs in edge cases; take everything from field 6 onward
            var value = string.Join("\t", fields.Skip(6));

            if (!string.IsNullOrEmpty(name))
            {
                cookies.Add($"{name}={value}");
            }
        }

        if (cookies.Count == 0)
            throw new FormatException($"No valid cookies found in file: {filePath}");

        return string.Join("; ", cookies);
    }
}
