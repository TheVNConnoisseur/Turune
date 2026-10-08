using System.Collections.Generic;
using Turune.Models;
using Turune.ViewModels;

namespace Turune.Services;

public static class MenuOptions
{
    public static IReadOnlyList<MenuOption> MainMenu (NavigationViewModel nav) => new List<MenuOption>
    {
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_V1_Name,
            Description = Localizations.Resources.MainMenu_V1_Description,
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "INI",
                    Description = Localizations.Resources.MainMenu_V1_INI_Description,
                    Children = MainMenu_V1_INI(nav),
                },
                new MenuOption
                {
                    Name = "ODN",
                    Description = Localizations.Resources.MainMenu_V1_ODN_Description,
                    Children = MainMenu_V1_ODN(nav),
                }
            },
        },
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_V2_Name,
            Description = Localizations.Resources.MainMenu_V2_Description,
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "DAT",
                    Description = "WIP",
                    Children = new List<MenuOption>
                    {
                        new MenuOption
                        {
                            Name = "Export",
                            Description = "test",
                            // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                        },
                    }
                },
                new MenuOption
                {
                    Name = "MG2",
                    Description = "WIP",
                },
                new MenuOption
                {
                    Name = "Testttt",
                    Description = "my lord",
                }
            }
        }
    };

    public static IReadOnlyList<MenuOption> MainMenu_V1_INI (NavigationViewModel nav) => new List<MenuOption>
    {
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_Operations_Export,
            Description = Localizations.Resources.MainMenu_V1_INI_Export_Description,
            // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
        },
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_Operations_Import,
            Description = Localizations.Resources.MainMenu_V1_INI_Import_Description,
            // Execute = () => nav.NavigateTo(new ExportIniViewModel(nav)),
        }
    };
    public static IReadOnlyList<MenuOption> MainMenu_V1_ODN (NavigationViewModel nav) => new List<MenuOption>
    {
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_Operations_Extract,
            Description = Localizations.Resources.MainMenu_V1_ODN_Extract_Description,
            // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
        },
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_Operations_Create,
            Description = Localizations.Resources.MainMenu_V1_ODN_Create_Description,
            // Execute = () => nav.NavigateTo(new ExportIniViewModel(nav)),
        }
    };
    public static IReadOnlyList<MenuOption> Configuration (NavigationViewModel nav) => new List<MenuOption>
    {
        //WIP, to let the user make some small configurations to the program, like the language perhaps
    };
}