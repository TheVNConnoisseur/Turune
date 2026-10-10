using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using Turune.ViewModels.Tools.Containers;

namespace Turune.Services;

public interface IToolViewModelFactory
{
    ObservableObject Create(string toolKey);
}

public sealed class ToolViewModelFactory(IServiceProvider services) : IToolViewModelFactory
{
    public ObservableObject Create(string toolKey) => toolKey switch
    {
        "ODN_Extract" => services.GetRequiredService<ODNExtractViewModel>(),
        // "ODN_Import" => services.GetRequiredService<ODNImportViewModel>(),
        _ => throw new ArgumentException($"Unknown tool key: '{toolKey}'", nameof(toolKey))
    };
}