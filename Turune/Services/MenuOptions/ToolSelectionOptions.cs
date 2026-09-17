using System;
using System.Collections.Generic;
using System.Text;
using Turune.Models;

namespace Turune.Services.MenuOptions
{
    public static class ToolSelectionOptions
    {
        public static IReadOnlyList<MenuOption> Version1 { get; } = new List<MenuOption>
        {
            new MenuOption {
                Key = "TEST 1",
                Description =
                {
                    new TextPart { Text = "TEST 1" },
                } 
            },
            new MenuOption {
                Key = "TEST 2",
                Description =
                {
                    new TextPart { Text = "TEST 2" },
                }
            },
        };

        //public static IReadOnlyList<MenuOption> Version2 { get; } = new List<MenuOption>
        //{
        //    new MenuOption { Key = "File Format",   Description = { "WIP" } },
        //    new MenuOption { Key = "Migration",     Description = { "WIP" } },
        //};
    }
}
