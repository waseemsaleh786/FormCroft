using System.Text;
using System.Text.RegularExpressions;

namespace FormBuilder.Application.Common;

/// <summary>Produces URL-friendly slugs from form names (used by the runtime route).</summary>
public static partial class SlugGenerator
{
    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Guid.NewGuid().ToString("n")[..8];

        var normalized = input.Trim().ToLowerInvariant();
        normalized = WhitespaceRegex().Replace(normalized, "-");
        normalized = InvalidCharsRegex().Replace(normalized, "");
        normalized = MultiDashRegex().Replace(normalized, "-").Trim('-');

        return string.IsNullOrEmpty(normalized)
            ? Guid.NewGuid().ToString("n")[..8]
            : normalized;
    }

    [GeneratedRegex(@"\s+")] private static partial Regex WhitespaceRegex();
    [GeneratedRegex(@"[^a-z0-9\-]")] private static partial Regex InvalidCharsRegex();
    [GeneratedRegex(@"\-{2,}")] private static partial Regex MultiDashRegex();
}
