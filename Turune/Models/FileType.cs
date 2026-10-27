using Avalonia.Platform.Storage;

namespace Turune.Models;

/// <summary>
/// Represents a custom FileType for a FilePicker. More on that here: https://docs.avaloniaui.net/docs/services/storage/file-picker-options#defining-custom-file-types
/// For AppleUniformTypeIdentifiers: https://developer.apple.com/documentation/uniformtypeidentifiers/defining-file-and-data-types-for-your-app
/// For MIMETypes: https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/MIME_types
/// </summary>
public class FileType()
{
    public class Container()
    {
        public static FilePickerFileType ODN { get; } = new("ODN container")
        {
            Patterns = ["*.odn"],
            AppleUniformTypeIdentifiers = ["public.archive"],
            MimeTypes = ["application/octet-stream"]
        };
    }

    public class Initialization()
    {
        public static FilePickerFileType DAT { get; } = new("Compiled initialization file")
        {
            Patterns = ["*.dat"],
            AppleUniformTypeIdentifiers = ["public.archive"],
            MimeTypes = ["application/octet-stream"]
        };
        public static FilePickerFileType INI { get; } = new("Initialization file")
        {
            Patterns = ["*.ini"],
            AppleUniformTypeIdentifiers = ["public.text"],
            MimeTypes = ["text/plain"]
        };
        public static FilePickerFileType INI_Compiled { get; } = new("Compiled initialization file")
        {
            Patterns = ["*.ini"],
            AppleUniformTypeIdentifiers = ["public.archive"],
            MimeTypes = ["application/octet-stream"]
        };
    }
}