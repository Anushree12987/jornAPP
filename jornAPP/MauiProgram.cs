using jornAPP.Components.Data;
using jornAPP.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.Charts;

namespace jornAPP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // MAUI Blazor
        builder.Services.AddMauiBlazorWebView();

        // App services
        builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<JournalService>();
        builder.Services.AddSingleton<SecurityService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<EntryService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<PdfService>();
        builder.Services.AddSyncfusionBlazor(); // ✅ Correct for MAUI Blazor


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}