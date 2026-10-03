using System.Collections.Generic;
using Turune.Models;

namespace Turune.Services.MenuOptions;

public static class MainMenuOptions
{
    public static IReadOnlyList<MenuOption> All { get; } = new List<MenuOption>
    {
        new MenuOption
        {
            Key = "Version 1",
            Description = """
                ## Supported games
                From [Imouto Ninshin](https://vndb.org/v2354) to 
                [Netoraren'ai ~Ai Suru Kanojo ga Musunda Midara na Keiyaku~](https://vndb.org/v2354).
                
                ## Compatibility
                * Before [Princess Knight☆Catue ~Ochita Ryuuki Hime~](https://vndb.org/v5067): probably supported, but the engine was still in
                development, so a lot of stuff probably underwent a lot of changes.
                * After [Princess Knight☆Catue ~Ochita Ryuuki Hime~](https://vndb.org/v5067): supported, but due to how the engine was developed,
                which was probably done very poorly, some edge cases might not be fully supported.
                """,
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
            Key = "Version 2",
            Description = """
                Starting from [Kangokujou ~Ingyaku no Shihai~](https://vndb.org/v20364), this is
                the latest main version of the MICo engine. It is important to understand that most
                of the files themselves have gone under several iterations in this same version.
                """,
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