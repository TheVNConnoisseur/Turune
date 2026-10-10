using System.Collections.Generic;
using Avalonia.Controls;
using Turune.Models;
using Turune.ViewModels;

namespace Turune.Services;

public static class MenuOptions
{
    public static IReadOnlyList<MenuOption> MainMenu () => new List<MenuOption>
    {
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_V1_Name,
            Description = Localizations.Resources.MainMenu_V1_Description,
            Children = MainMenu_V1()
        },
        new MenuOption
        {
            Name = Localizations.Resources.MainMenu_V2_Name,
            Description = Localizations.Resources.MainMenu_V2_Description,
            Children = MainMenu_V2()
        }
    };

    public static IReadOnlyList<MenuOption> MainMenu_V1 () => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "Images",
            Description = "Extensionless files, they hold any kind of images.",
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "Import",
                    Description = "Convert images in common formats to game-ready images.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                },
                new MenuOption
                {
                    Name = "Export",
                    Description = "Convert game-ready images to a common format.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                }
            }
        },
        new MenuOption
        {
            Name = "INI",
            Description = Localizations.Resources.MainMenu_V1_INI_Description,
            Children = new List<MenuOption>
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
            }
        },
        new MenuOption
        {
            Name = "ODN",
            Description = Localizations.Resources.MainMenu_V1_ODN_Description,
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = Localizations.Resources.MainMenu_Operations_Extract,
                    Description = Localizations.Resources.MainMenu_V1_ODN_Extract_Description,
                    ToolKey = "ODN_Extract"
                },
                new MenuOption
                {
                    Name = Localizations.Resources.MainMenu_Operations_Create,
                    Description = Localizations.Resources.MainMenu_V1_ODN_Create_Description,
                    ToolKey = "ODN_Create"
                }
            }
        }
    };
    
    public static IReadOnlyList<MenuOption> MainMenu_V2 () => new List<MenuOption>
    {
        new MenuOption
        {
            Name = "AM2",
            Description = "Animated images consisting of layers of multiple MG2 images.",
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "Create",
                    Description = "Combine 2 or more MG2 files to create an animated image."
                },
                new MenuOption
                {
                    Name = "Extract",
                    Description = "Obtain all of the MG2 files that are inside an AM2 container."
                }
            }
        },
        new MenuOption
        {
            Name = "DAT",
            Description = "The containers of the files.",
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "Extract",
                    Description = "extract the files from the dat files.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                },
            }
        },
        new MenuOption
        {
            Name = "MAL",
            Description = "Mask images used for transitions.",
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "Create",
                    Description = "Create 1 or more mask image(s) from a standard image format.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                },
                new MenuOption
                {
                    Name = "Export",
                    Description = "Convert any given amount of standard images to game-ready MAL files.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                }
            }
        },
        new MenuOption
        {
            Name = "MG2",
            Description = "Images",
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "Create",
                    Description = "Create 1 or more MG2 image(s) from a standard image format.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                },
                new MenuOption
                {
                    Name = "Export",
                    Description = "Convert any given amount of standard images to game-ready MG2 files.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                }
            }
        },
        new MenuOption
        {
            Name = "SDT",
            Description = "Compiled script files, they are protected.",
            Children = new List<MenuOption>
            {
                new MenuOption
                {
                    Name = "Compile",
                    Description = "Create compiled script files from previously decompiled script files.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                },
                new MenuOption
                {
                    Name = "Decompile",
                    Description = "Convert compiled script files to human-readable text files.",
                    // Execute = () => nav.NavigateTo(new LoadIniViewModel(nav)),
                }
            }
        }
    };
    
    public static IReadOnlyList<MenuOption> Configuration (NavigationViewModel nav) => new List<MenuOption>
    {
        //WIP, to let the user make some small configurations to the program, like the language perhaps
    };
}