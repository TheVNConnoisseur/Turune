using System;
using System.Collections.Generic;

namespace Turune.Models;

/// <summary>
/// Represents an option in the main menu.
/// </summary>
public class MenuOption
{
    /// <summary>
    /// Human-readable name of the option, displayed in the menu. This is what the user
    /// is supposed to see in the program's interface.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Human-readable description of the option, written in Markdown.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Sub-options, allowing for nested menus.
    /// </summary>
    public IReadOnlyList<MenuOption> Children { get; set; } = new List<MenuOption>();

    /// <summary>
    /// The action to be executed when the option is selected. This can be a method, a delegate, or any other callable entity.
    /// </summary>
    public Action? Execute { get; set; }

    /// <summary>
    /// Indicates whether the option has any sub-options (children).
    /// </summary>
    /// <value>True if the option has children; otherwise, false.</value>
    public bool HasChildren => Children.Count > 0;
}
