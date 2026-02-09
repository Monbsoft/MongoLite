using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using Monbsoft.MongoLite.MApp.Services;
using Monbsoft.MongoLite.MApp.ViewModels;
// using Monbsoft.MongoLite.MApp.Pages; // Commented temporarily

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
        
        // Register ViewModels
        builder.Services.AddTransient<ConnectionViewModel>();
        builder.Services.AddTransient<CollectionsViewModel>();
        builder.Services.AddTransient<DocumentsViewModel>();
        builder.Services.AddTransient<DocumentDetailViewModel>();
        
        // Register Pages (to be added later)
        // builder.Services.AddTransient<ConnectionPage>();
        // builder.Services.AddTransient<CollectionsPage>();
        // builder.Services.AddTransient<DocumentsPage>();
        // builder.Services.AddTransient<DocumentDetailPage>();

        return builder.Build();
    }
}
