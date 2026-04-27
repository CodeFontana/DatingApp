namespace Client.Features.Theme;

public static class AppTheme
{
    private static readonly string[] s_sansSerifStack =
    [
        "Inter",
        "system-ui",
        "-apple-system",
        "Segoe UI",
        "Roboto",
        "Helvetica Neue",
        "Arial",
        "sans-serif"
    ];

    private static readonly string[] s_monoStack =
    [
        "JetBrains Mono",
        "ui-monospace",
        "SFMono-Regular",
        "Menlo",
        "Consolas",
        "monospace"
    ];

    public static readonly MudTheme Default = new()
    {
        PaletteLight = BuildLightPalette(),
        PaletteDark = BuildDarkPalette(),
        Typography = BuildTypography(),
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "260px",
            AppbarHeight = "64px"
        }
    };

    /// <summary>
    /// Light palette inspired by Bootswatch United: Ubuntu orange primary,
    /// aubergine purple "Dark", soft GitHub-style surfaces.
    /// </summary>
    private static PaletteLight BuildLightPalette() => new()
    {
        Black = "#1F2328",
        White = "#FFFFFF",

        Primary = "#E95420",
        PrimaryContrastText = "#FFFFFF",
        PrimaryDarken = "#BE4013",
        PrimaryLighten = "#F47C50",

        Secondary = "#AD5276",
        SecondaryContrastText = "#FFFFFF",
        SecondaryDarken = "#7C3A55",
        SecondaryLighten = "#C5839D",

        Tertiary = "#2C9FAA",
        TertiaryContrastText = "#FFFFFF",
        TertiaryDarken = "#1F767E",
        TertiaryLighten = "#50C0CB",

        Info = "#3B82F6",
        InfoContrastText = "#FFFFFF",
        InfoDarken = "#2563EB",
        InfoLighten = "#60A5FA",

        Success = "#16A34A",
        SuccessContrastText = "#FFFFFF",
        SuccessDarken = "#0F7F39",
        SuccessLighten = "#3FBD6B",

        Warning = "#F59E0B",
        WarningContrastText = "#1F2328",
        WarningDarken = "#D97706",
        WarningLighten = "#FBBF24",

        Error = "#DC2626",
        ErrorContrastText = "#FFFFFF",
        ErrorDarken = "#B91C1C",
        ErrorLighten = "#EF4444",

        Dark = "#772953",
        DarkContrastText = "#FFFFFF",
        DarkDarken = "#5D2040",
        DarkLighten = "#9D3870",

        Background = "#F6F8FA",
        BackgroundGray = "#EAEEF2",
        Surface = "#FFFFFF",

        AppbarBackground = "#FFFFFF",
        AppbarText = "#1F2328",

        DrawerBackground = "#FFFFFF",
        DrawerText = "#1F2328",
        DrawerIcon = "#57606A",

        TextPrimary = "#1F2328",
        TextSecondary = "#57606A",
        TextDisabled = "#8C959F",

        ActionDefault = "#57606A",
        ActionDisabled = "#BBC3CB",
        ActionDisabledBackground = "#EAEEF2",

        LinesDefault = "#D0D7DE",
        LinesInputs = "#8C959F",
        TableLines = "#EAEEF2",
        TableStriped = "#F6F8FA",
        TableHover = "#EAEEF2",

        Divider = "#D0D7DE",
        DividerLight = "#EAEEF2",

        GrayDefault = "#57606A",
        GrayLight = "#8C959F",
        GrayLighter = "#D0D7DE",
        GrayDark = "#424A53",
        GrayDarker = "#1F2328",

        OverlayDark = "rgba(15, 23, 42, 0.6)",
        OverlayLight = "rgba(255, 255, 255, 0.6)",

        HoverOpacity = 0.06,
        RippleOpacity = 0.10,
        RippleOpacitySecondary = 0.20
    };

    /// <summary>
    /// Dark palette modelled on GitHub "Dark Dimmed" so it stays soft on the
    /// eyes, with a brand-orange primary and a complementary lifted rose for
    /// secondary so the United character carries through.
    /// </summary>
    private static PaletteDark BuildDarkPalette() => new()
    {
        Black = "#1C2128",
        White = "#CDD9E5",

        Primary = "#FF8857",
        PrimaryContrastText = "#1C2128",
        PrimaryDarken = "#E95420",
        PrimaryLighten = "#FFB088",

        Secondary = "#C5839D",
        SecondaryContrastText = "#1C2128",
        SecondaryDarken = "#AD5276",
        SecondaryLighten = "#DDA8C0",

        Tertiary = "#4FB6BF",
        TertiaryContrastText = "#1C2128",
        TertiaryDarken = "#2C9FAA",
        TertiaryLighten = "#79CDD3",

        Info = "#539BF5",
        InfoContrastText = "#CDD9E5",
        InfoDarken = "#316DCA",
        InfoLighten = "#6CB6FF",

        Success = "#57AB5A",
        SuccessContrastText = "#CDD9E5",
        SuccessDarken = "#347D39",
        SuccessLighten = "#7EBA80",

        Warning = "#C69026",
        WarningContrastText = "#1C2128",
        WarningDarken = "#966600",
        WarningLighten = "#DAAA3F",

        Error = "#E5534B",
        ErrorContrastText = "#CDD9E5",
        ErrorDarken = "#C93C37",
        ErrorLighten = "#F47067",

        Dark = "#1C2128",
        DarkContrastText = "#ADBAC7",
        DarkDarken = "#13171D",
        DarkLighten = "#2D333B",

        Background = "#22272E",
        BackgroundGray = "#2D333B",
        Surface = "#2D333B",

        AppbarBackground = "#1C2128",
        AppbarText = "#ADBAC7",

        DrawerBackground = "#1C2128",
        DrawerText = "#ADBAC7",
        DrawerIcon = "#768390",

        TextPrimary = "#ADBAC7",
        TextSecondary = "#768390",
        TextDisabled = "#636E7B",

        ActionDefault = "#ADBAC7",
        ActionDisabled = "#636E7B",
        ActionDisabledBackground = "#373E47",

        LinesDefault = "#444C56",
        LinesInputs = "#444C56",
        TableLines = "#373E47",
        TableStriped = "#2D333B",
        TableHover = "#373E47",

        Divider = "#373E47",
        DividerLight = "#2D333B",

        GrayDefault = "#768390",
        GrayLight = "#636E7B",
        GrayLighter = "#444C56",
        GrayDark = "#ADBAC7",
        GrayDarker = "#CDD9E5",

        OverlayDark = "rgba(13, 17, 23, 0.7)",
        OverlayLight = "rgba(34, 39, 46, 0.6)",

        HoverOpacity = 0.08,
        RippleOpacity = 0.12,
        RippleOpacitySecondary = 0.22
    };

    private static Typography BuildTypography() => new()
    {
        Default = new DefaultTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.9rem",
            FontWeight = "400",
            LineHeight = "1.55",
            LetterSpacing = "normal"
        },
        H1 = new H1Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "3rem",
            FontWeight = "700",
            LineHeight = "1.1",
            LetterSpacing = "-0.025em"
        },
        H2 = new H2Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "2.25rem",
            FontWeight = "700",
            LineHeight = "1.15",
            LetterSpacing = "-0.02em"
        },
        H3 = new H3Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1.75rem",
            FontWeight = "700",
            LineHeight = "1.2",
            LetterSpacing = "-0.015em"
        },
        H4 = new H4Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1.375rem",
            FontWeight = "600",
            LineHeight = "1.3",
            LetterSpacing = "-0.01em"
        },
        H5 = new H5Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1.125rem",
            FontWeight = "600",
            LineHeight = "1.4",
            LetterSpacing = "-0.005em"
        },
        H6 = new H6Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1rem",
            FontWeight = "600",
            LineHeight = "1.45",
            LetterSpacing = "normal"
        },
        Subtitle1 = new Subtitle1Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.95rem",
            FontWeight = "500",
            LineHeight = "1.5",
            LetterSpacing = "normal"
        },
        Subtitle2 = new Subtitle2Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.85rem",
            FontWeight = "500",
            LineHeight = "1.5",
            LetterSpacing = "normal"
        },
        Body1 = new Body1Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.95rem",
            FontWeight = "400",
            LineHeight = "1.6",
            LetterSpacing = "normal"
        },
        Body2 = new Body2Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.85rem",
            FontWeight = "400",
            LineHeight = "1.55",
            LetterSpacing = "normal"
        },
        Button = new ButtonTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.85rem",
            FontWeight = "600",
            LineHeight = "1.75",
            LetterSpacing = "0.01em",
            TextTransform = "none"
        },
        Caption = new CaptionTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.75rem",
            FontWeight = "400",
            LineHeight = "1.4",
            LetterSpacing = "0.02em"
        },
        Overline = new OverlineTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.7rem",
            FontWeight = "600",
            LineHeight = "1.5",
            LetterSpacing = "0.12em",
            TextTransform = "uppercase"
        }
    };
}
