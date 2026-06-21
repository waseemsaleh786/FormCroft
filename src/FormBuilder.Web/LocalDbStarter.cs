using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FormBuilder.Web;

/// <summary>
/// Ensures a named SQL Server LocalDB instance is started before the app connects.
/// LocalDB stops named (non‑auto‑create) instances after idle/reboot and they do not
/// always auto‑start on connect, which surfaces as "The specified LocalDB instance does
/// not exist". Running <c>sqllocaldb start</c> on launch makes <c>dotnet run</c> self‑sufficient.
/// No‑op on non‑Windows or when the connection string isn't a LocalDB one.
/// </summary>
public static partial class LocalDbStarter
{
    public static void EnsureStarted(string? connectionString, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        var match = LocalDbInstanceRegex().Match(connectionString);
        if (!match.Success) return;

        var instance = match.Groups["name"].Value;
        if (string.IsNullOrWhiteSpace(instance)) return; // automatic instance handles itself

        try
        {
            var psi = new ProcessStartInfo("sqllocaldb", $"start \"{instance}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            process?.WaitForExit(15000);
            logger.LogInformation("Ensured LocalDB instance '{Instance}' is started.", instance);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not auto-start LocalDB instance '{Instance}'. Start it manually if connection fails.", instance);
        }
    }

    // Matches Server=(localdb)\InstanceName  (instance optional for the automatic instance)
    [GeneratedRegex(@"\(localdb\)\\(?<name>[^;]*)", RegexOptions.IgnoreCase)]
    private static partial Regex LocalDbInstanceRegex();
}
