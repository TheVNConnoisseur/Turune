using System.Collections.Generic;
using Turune.Models;
using Turune.ViewModels;

namespace Turune.Services.MenuOptions;

public static partial class MenuOptions
{
    public static IReadOnlyList<MenuOption> MainMenu_V1 => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "INI",
            Description = Localizations.Resources.MainMenu_V1_INI_Description,
            // Execute = () => navigation.NavigateTo(new MainMenuViewModel(_nav, "1", "", isRootMenu: false)),
        },
        new MenuOption
        {
            Name = "ODN",
            Description = Localizations.Resources.MainMenu_V1_ODN_Description,
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
            Description = Localizations.Resources.MainMenu_V2_Description
        }
    };

    public static IReadOnlyList<MenuOption> Configuration => new List<MenuOption>
    {
        //WIP, to let the user make some small configurations to the program, like the language perhaps
    };
}