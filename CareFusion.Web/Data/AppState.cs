// Placeholder for Data/AppState.cs
namespace CareFusion.Web.Data;

public class AppState
{
    public string UiLanguage { get; private set; } = "en";
    public event Action? Changed;

    public void SetLanguage(string lang) { UiLanguage = lang; Changed?.Invoke(); }
}
