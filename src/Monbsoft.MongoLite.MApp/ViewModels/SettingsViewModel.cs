using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private bool _isDarkTheme;

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                ApplyTheme();
                OnPropertyChanged(nameof(ThemeLabel));
            }
        }
    }

    public string ThemeLabel => IsDarkTheme ? "Sombre" : "Clair";

    public SettingsViewModel()
    {
        _isDarkTheme = Application.Current?.UserAppTheme == AppTheme.Dark;
    }

    private void ApplyTheme()
    {
        if (Application.Current is null) return;

        Application.Current.UserAppTheme = IsDarkTheme ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set("AppTheme", Application.Current.UserAppTheme.ToString());
    }
}
