using Microsoft.Extensions.Logging;
using Monbsoft.MongoLite.MApp.Pages;
using Monbsoft.MongoLite.MApp.Services;
using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<MongoDbService>();
        builder.Services.AddSingleton<ConnectionStorageService>();

        // Register ViewModels
        builder.Services.AddTransient<ConnectionViewModel>();
        builder.Services.AddTransient<AdvancedConnectionViewModel>();
        builder.Services.AddTransient<CollectionsViewModel>();
        builder.Services.AddTransient<DocumentsViewModel>();
        builder.Services.AddTransient<DocumentDetailViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Register Pages
        builder.Services.AddTransient<ConnectionPage>();
        builder.Services.AddTransient<AdvancedConnectionPage>();
        builder.Services.AddTransient<CollectionsPage>();
        builder.Services.AddTransient<DocumentsPage>();
        builder.Services.AddTransient<DocumentDetailPage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
    }
}
