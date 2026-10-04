using System.Collections.Generic;
using Turune.Models;
using Turune.ViewModels;

namespace Turune.Services.MenuOptions;

public static partial class MenuOptions
{
    public static IReadOnlyList<MenuOption> V1 => new List<MenuOption>
    {
        // new MenuOption
        // {
        //     Name = Localizations.Resources.MainMenu_V1_Name,
        //     Description = Localizations.Resources.MainMenu_V1_Description,
        //     Children = MainMenu_V1,
        // },
        // new MenuOption
        // {
        //     Name = Localizations.Resources.MainMenu_V2_Name,
        //     Description = Localizations.Resources.MainMenu_V2_Description
        // }
    };
}