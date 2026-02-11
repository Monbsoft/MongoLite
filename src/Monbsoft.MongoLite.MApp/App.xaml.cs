namespace Monbsoft.MongoLite.MApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        LoadThemePreference();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private void LoadThemePreference()
    {
        var savedTheme = Preferences.Get("AppTheme", string.Empty);
        if (Enum.TryParse<AppTheme>(savedTheme, out var theme))
            UserAppTheme = theme;
    }
}
