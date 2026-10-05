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
                    Children = V1_INI(nav),
                },
                new MenuOption
                {
                    Name = "ODN",
                    Description = Localizations.Resources.MainMenu_V1_ODN_Description,
                    Children = V1_ODN(nav),
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

    public static IReadOnlyList<MenuOption> V1_INI (NavigationViewModel nav) => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "working on it",
            Description = "maybe tomorrow",
            Execute = () => nav.NavigateTo(new NavigationViewModel()),
        }
    };
    public static IReadOnlyList<MenuOption> V1_ODN (NavigationViewModel nav) => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "not working on it",
            Description = "maybe tomorrow",
        }
    };
    public static IReadOnlyList<MenuOption> Configuration (NavigationViewModel nav) => new List<MenuOption>
    {
        //WIP, to let the user make some small configurations to the program, like the language perhaps
    };
}