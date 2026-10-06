using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Turune.Services;
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
            var storageService = new StorageService(() => desktop.MainWindow?.StorageProvider);
            var nav = new NavigationViewModel();

            nav.NavigateTo(new MainMenuViewModel(
                Localizations.Resources.MainMenu_Title,
                MenuOptions.MainMenu(nav),
                storageService));

            desktop.MainWindow = new MainWindow
            {
                DataContext = nav
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}