using System;
using System.Collections.Generic;

namespace Turune.Models
{
    public class MenuOption
    {
        public required string Key { get; set; }
        public string Description { get; set; } = string.Empty;     //In Markdown
        
        public List<MenuOption> Children { get; set; } = new(); //Sub-options, allowing for nested menus.

        public bool HasChildren => Children.Count > 0;

        public Action? Execute { get; set; }
    }
}
