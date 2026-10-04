using System.Collections.Generic;
using Turune.Models;
using Turune.ViewModels;

namespace Turune.Services;

public static class MenuOptions
{
    public static IReadOnlyList<MenuOption> MainMenu_V1 => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "INI",
            Description = Localizations.Resources.MainMenu_V1_INI_Description,
            Children = V1_INI,
        },
        new MenuOption
        {
            Name = "ODN",
            Description = Localizations.Resources.MainMenu_V1_ODN_Description,
            Children = V1_ODN,
        }
    };
    public static IReadOnlyList<MenuOption> MainMenu_V2 => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "DAT",
            Description = "WIP",
            // Execute = () => navigation.NavigateTo(new MainMenuViewModel(_nav, "1", "", isRootMenu: false)),
        },
        new MenuOption
        {
            Name = "Yikes not done yet",
            Description = "WIP",
        },
        new MenuOption
        {
            Name = "Testttt",
            Description = "my lord",
        }
    };
    public static IReadOnlyList<MenuOption> MainMenu => new List<MenuOption>
    {
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_V1_Name,
            Description = Localizations.Resources.MainMenu_V1_Description,
            Children = MainMenu_V1,
        },
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_V2_Name,
            Description = Localizations.Resources.MainMenu_V2_Description,
            Children = MainMenu_V2,
        }
    };

    public static IReadOnlyList<MenuOption> V1_INI => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "working on it",
            Description = "maybe tomorrow",
        }
    };
    public static IReadOnlyList<MenuOption> V1_ODN => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "not working on it",
            Description = "maybe tomorrow",
        }
    };
    public static IReadOnlyList<MenuOption> Configuration => new List<MenuOption>
    {
        //WIP, to let the user make some small configurations to the program, like the language perhaps
    };
}