using System.Collections.Generic;
using Turune.Models;

namespace Turune.Services.MenuOptions
{
    public static class MainMenuOptions
    {
        public static IReadOnlyList<MenuOption> All { get; } = new List<MenuOption>
        {
            new MenuOption
            {
                Key = "Version 1",
                Description =
                {
                    new TextPart { Text = "For the games released in between " },
                    new LinkPart { Text = "Imouto Ninshin", Url = "https://vndb.org/v2354" },
                    new TextPart { Text = " and " },
                    new LinkPart { Text = "Netoraren'ai ~Ai Suru Kanojo ga Musunda Midara na Keiyaku~",
                                   Url = "https://vndb.org/v2354" },
                    new TextPart { Text = " (both included)." },
                    new LineBreakPart(),
                    new TextPart { Text = "Compatibility for the earlier games is not guaranteed, " +
                                           "since the engine went under a lot of changes during " +
                                           "the initial days of Valkyria." }
                },
                Children =
                {
                    new MenuOption
                    {
                        Key = "Compatibility Notes",
                        Description =
                        {
                            new TextPart { Text = "Some earlier titles may not load save files " +
                                                   "from Version 1. Use a fresh install if you " +
                                                   "run into issues." }
                        }
                    },
                    new MenuOption
                    {
                        Key = "Known Issues",
                        Description =
                        {
                            new TextPart { Text = "See the " },
                            new LinkPart { Text = "issue tracker", Url = "https://example.com/issues" },
                            new TextPart { Text = " for the current list." }
                        }
                    }
                }
            },
            new MenuOption
            {
                Key = "Version 2",
                Description =
                {
                    new TextPart { Text = "Starting from " },
                    new LinkPart { Text = "Kangokujou ~Ingyaku no Shihai~",
                                               Url = "https://vndb.org/v20364" },
                    new TextPart { Text = ", this is the latest main version of the MICo engine. " +
                                           "It is important to understand that most of the files " +
                                           "themselves have gone under several iterations in this " +
                                           "same version." }
                },
                Children =
                {
                    new MenuOption
                    {
                        Key = "File Format",
                        Description =
                        {
                            new TextPart { Text = "Version 2 introduced a new packaging format. " +
                                                   "Older tools may need an update to read it." }
                        }
                    },
                    new MenuOption
                    {
                        Key = "Migration Guide",
                        Description =
                        {
                            new TextPart { Text = "How to move an existing install to Version 2. " },
                            new LinkPart { Text = "Full guide", Url = "https://example.com/migrate" }
                        }
                    }
                }
            }
        };
    }
}