using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Turune;

//Taken from: https://docs.avaloniaui.net/docs/how-to/navigation-how-to#view-locator-pattern
public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null) return new TextBlock { Text = "No data" };

        var name = data.GetType().FullName!
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = Type.GetType(name);

        return type is not null
            ? (Control)Activator.CreateInstance(type)!
            : new TextBlock { Text = $"View not found: {name}" };
    }

    public bool Match(object? data) => data is ObservableObject;
}