using MudBlazor;

namespace FormBuilder.Web.Theme;

/// <summary>
/// MudBlazor theme tuned to the FormCraft mockups: lime-green accent (#CBEC00)
/// with dark-grey text on a light-grey canvas, Inter typeface, rounded corners.
/// </summary>
public static class FormCraftTheme
{
    public const string Green = "#CBEC00";
    public const string DarkGrey = "#2A2A2A";
    public const string Grey = "#5A5A5A";
    public const string LightGrey = "#F5F5F5";
    public const string Border = "#E5E7EB";
    public const string Red = "#DC2626";
    public const string Blue = "#3B82F6";

    public static readonly MudTheme Instance = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = Green,
            PrimaryContrastText = DarkGrey,
            Secondary = DarkGrey,
            Tertiary = Blue,
            Background = LightGrey,
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = DarkGrey,
            DrawerBackground = "#FFFFFF",
            TextPrimary = DarkGrey,
            TextSecondary = Grey,
            ActionDefault = Grey,
            Error = Red,
            LinesDefault = Border,
            TableLines = Border,
            Divider = Border,
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = new[] { "Inter", "-apple-system", "sans-serif" } }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px"
        }
    };
}
