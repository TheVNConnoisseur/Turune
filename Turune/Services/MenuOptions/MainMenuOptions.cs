using System.Collections.Generic;
using Turune.Models;

namespace Turune.Services.MenuOptions;

public static class MainMenuOptions
{
    public static IReadOnlyList<MenuOption> All { get; } = new List<MenuOption>
    {
        new MenuOption
        {
            Key = Localizations.Resources.MainMenu_Version1,
            Description = Localizations.Resources.MainMenu_Version1Description,
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Key = "INI",
                    Description = """
                        The initialization file for the game doesn't really do much besides changing
                        the game's title, since we don't have access to any debug feature.
                        """
                },
                new MenuOption
                {
                    Key = "ODN",
                    Description = """
                        The container files that store all of the game's assets. Some of them are
                        encrypted, and it is hardcoded by what kind of asset they store.
                        """
                }
            }
        },
        new MenuOption
        {
            Key = Localizations.Resources.MainMenu_Version2,
            Description = Localizations.Resources.MainMenu_Version2Description,
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Key = "File Format",
                    Description = """
                        Version 2 introduced a new packaging format. Older tools may need an update to read it.
                        """
                },
                new MenuOption
                {
                    Key = "Migration Guide",
                    Description = """
                        How to move an existing install to Version 2.
                        [Full guide](https://example.com/migrate)
                        """
                }
            }
        }
    };
}