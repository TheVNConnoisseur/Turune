using Microsoft.Win32;

namespace Turune.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string? OpenFile(
            string? title = null,
            string? filter = null,
            string? initialDirectory = null)
        {
            var dialog = new OpenFileDialog
            {
                Title = title ?? "Open File",
                Filter = filter ?? "All files (*.*)|*.*"
            };

            if (!string.IsNullOrEmpty(initialDirectory))
                dialog.InitialDirectory = initialDirectory;

            return dialog.ShowDialog() == true
                ? dialog.FileName
                : null;
        }

        public string[]? OpenFiles(
            string? title = null,
            string? filter = null,
            string? initialDirectory = null)
        {
            var dialog = new OpenFileDialog
            {
                Title = title ?? "Open File",
                Filter = filter ?? "All files (*.*)|*.*",
                Multiselect = true
            };

            if (!string.IsNullOrEmpty(initialDirectory))
                dialog.InitialDirectory = initialDirectory;

            return dialog.ShowDialog() == true
                ? dialog.FileNames
                : null;
        }

        public string? SaveFile(
            string? title = null,
            string? filter = null,
            string? initialDirectory = null,
            string? defaultFileName = null)
        {
            var dialog = new SaveFileDialog
            {
                Title = title ?? "Save File",
                Filter = filter ?? "All files (*.*)|*.*",
                FileName = defaultFileName ?? ""
            };

            if (!string.IsNullOrEmpty(initialDirectory))
                dialog.InitialDirectory = initialDirectory;

            return dialog.ShowDialog() == true
                ? dialog.FileName
                : null;
        }

        public string? SelectFolder(
            string? title = null,
            string? initialDirectory = null)
        {
            var dialog = new OpenFolderDialog
            {
                Title = title ?? "Select Folder"
            };

            if (!string.IsNullOrEmpty(initialDirectory))
                dialog.InitialDirectory = initialDirectory;

            return dialog.ShowDialog() == true
                ? dialog.FolderName
                : null;
        }
    }
}
