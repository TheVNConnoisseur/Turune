using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Turune.Services.MenuOptions;
using Turune.ViewModels;
using Turune.Views;

namespace Turune;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var nav = new NavigationViewModel();
            nav.NavigateTo(new MainMenuViewModel(nav, Localizations.Resources.SelectEngineRevision, MenuOptions.MainMenu, isRootMenu: true));

            desktop.MainWindow = new MainWindow
            {
                DataContext = nav
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}