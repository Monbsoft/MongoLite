namespace Monbsoft.MongoLite.MApp.Models;

public enum EnvironmentType
{
    Development,
    Staging,
    Production,
    Testing,
    Other
}

public static class EnvironmentExtensions
{
    public static string GetColor(this EnvironmentType environment) => environment switch
    {
        EnvironmentType.Development => "#FF4CAF50", // Green
        EnvironmentType.Staging => "#FFFFC107", // Amber/Yellow
        EnvironmentType.Production => "#FFF44336", // Red
        EnvironmentType.Testing => "#FF2196F3", // Blue
        _ => "#FF9E9E9E" // Gray
    };

    public static string GetDisplayName(this EnvironmentType environment) => environment switch
    {
        EnvironmentType.Development => "Development",
        EnvironmentType.Staging => "Staging",
        EnvironmentType.Production => "Production",
        EnvironmentType.Testing => "Testing",
        _ => "Other"
    };
}
