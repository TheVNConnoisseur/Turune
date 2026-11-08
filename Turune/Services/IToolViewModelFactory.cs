using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Turune.Services;

public interface IToolViewModelFactory
{
    ObservableObject Create(string toolKey);
}

public sealed class ToolViewModelFactory(IServiceProvider services) : IToolViewModelFactory
{
    public ObservableObject Create(string toolKey) => toolKey switch
    {
        "Containers_ODN_Create" => services.GetRequiredService<ViewModels.Tools.Containers.ODNCreateViewModel>(),
        "Containers_ODN_Extract" => services.GetRequiredService<ViewModels.Tools.Containers.ODNExtractViewModel>(),
        "Images_RAW_Export" => services.GetRequiredService<ViewModels.Tools.Images.RAWExportViewModel>(),
        "Images_RAW_Import" => services.GetRequiredService<ViewModels.Tools.Images.RAWImportViewModel>(),
        "Initialization_DAT_Export" => services.GetRequiredService<ViewModels.Tools.Initialization.DATExportViewModel>(),
        "Initialization_DAT_Import" => services.GetRequiredService<ViewModels.Tools.Initialization.DATImportViewModel>(),
        "Initialization_INI_Export" => services.GetRequiredService<ViewModels.Tools.Initialization.INIExportViewModel>(),
        "Initialization_INI_Import" => services.GetRequiredService<ViewModels.Tools.Initialization.INIImportViewModel>(),
        "Scripts_SDT_Decompile" => services.GetRequiredService<ViewModels.Tools.Scripts.SDTDecompileViewModel>(),
        _ => throw new ArgumentException($"Unknown tool key: '{toolKey}'", nameof(toolKey))
    };
}