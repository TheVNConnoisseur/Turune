# Turune

**Turune** is a toolset enabling modders to modify files used in the propietary engine called **MICo**, used in visual novels produced by [VALKYRIA](https://vndb.org/p765).

## Roadmap

- Version 1
  - [ ] Images
    - [ ] Import
    - [ ] Extract
  - [ ] INI
    - [ ] Export
    - [ ] Import
  - [ ] ODN
    -  [x] Extract
    -  [x] Create

- Version 2
  - [ ] AM2
    - [ ] Create
    - [ ] Extract
  -  [ ] DAT
    -  [ ] Extract
  - [ ] MAL
    - [ ] Create
    - [ ] Export
  - [ ] MG2
    - [ ] Create
    - [ ] Export
  - [ ] SDT
    - [ ] Compile
    - [ ] Decompile

:information_source: If there is something not listed here, it means that it will be ***NOT*** supported, because either is pointless or doesn't make sense in the context of the tool.

To check out how's the progress in understanding each of file formats available in their engine, check out the [MICo_Guide](https://github.com/TheVNConnoisseur/MICo_Guide/) repository.

## Building

:warning: Make sure to read which platforms are supported in the [supported platforms](#Supported-platforms) subsection.

### Requirements

#### Mandatory

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- An IDE with MSBuild or equivalent support:
  - [Visual Studio](https://visualstudio.microsoft.com/downloads/): install the **.NET desktop environment**.
  - [Visual Studio Code](https://code.visualstudio.com/download): install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension.
  - [VSCodium](https://github.com/VSCodium/vscodium) (or forks of it): install the [SharpLsp](https://sharplsp.dev/) or [ReSharper](https://www.jetbrains.com/resharper/) extensions. Keep in mind that none of these are able to work with the official Avalonia Extension, due to it putting C# Dev Kit as a dependency.

#### Optional

- Avalonia Extension for [VS Code](https://avaloniaui.net/vscode) or [Visual Studio](https://marketplace.visualstudio.com/items?itemName=AvaloniaTeam.AvaloniaVS) *(only if you plan on developing/debugging any feature)*
- [PupNet Deploy](https://github.com/kuiperzone/PupNet-Deploy) (uses to build the application ready to be distributed)


### Supported platforms

| Name          | Status             | Comments |
| :-----------: | :-----------:      | :-----------: |
| Windows       | :white_check_mark: | - |
| Mac           | :grey_question:    | Should work in theory, but it has not been tested. |
| Linux         | :white_check_mark: | - |
| WebAssembly   | :x:                | All API calls that expect an entire file system, like `Directories`, are not designed to work with browsers. |


### Deployment

In order to make the deployment of any new version easy, [PupNet Deploy](https://github.com/kuiperzone/PupNet-Deploy) was chosen as the framework for making the process of creating new releases less tedious.

For that reason alone, it's important to read the instructions in their README file to ensure everything works.


## Licenses

[Bootstrap Icons](https://github.com/twbs/icons) - MIT License

[Markdown.Avalonia.Tight](https://www.nuget.org/packages/Markdown.Avalonia.Tight) - MIT License

[Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) - MIT License

[NVorbis](https://github.com/NVorbis/NVorbis) - MIT License

[Outfit Fonts](https://github.com/Outfitio/Outfit-Fonts) - SIL Open Font License 1.1

[PupNet Deploy](https://github.com/kuiperzone/PupNet-Deploy) - GNU Affero General Public License 3.0