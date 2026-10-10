using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Turune.Services;
using Turune.ViewModels;
using Turune.ViewModels.Tools.Containers;
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
            //Creating a ServiceCollection in order to have all dependencies together
            var services = new ServiceCollection();

            //The core services will singletons, to ensure that
            services.AddSingleton<IStorageService>(_ =>
                new StorageService(() => desktop.MainWindow?.StorageProvider));
            services.AddSingleton<IFileLoader, FileLoader>();
            services.AddSingleton<NavigationViewModel>();
            services.AddSingleton<IToolViewModelFactory, ToolViewModelFactory>();

            //Transient ViewModels, no need to keep their state
            services.AddTransient<ODNExtractViewModel>();

            var provider = services.BuildServiceProvider();

            var nav = provider.GetRequiredService<NavigationViewModel>();

            nav.NavigateTo(new MainMenuViewModel(
                Localizations.Resources.MainMenu_Title,
                MenuOptions.MainMenu(),
                nav,
                provider.GetRequiredService<IToolViewModelFactory>()));

            desktop.MainWindow = new MainWindow
            {
                DataContext = nav
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}