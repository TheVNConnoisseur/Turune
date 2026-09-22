namespace Turune.Services
{
    public interface IFileDialogService //Abstraction layer included just in case somebody wants to port it to another framework
    {
        string? OpenFile(
        string? title = null,
        string? filter = null,
        string? initialDirectory = null);

        string[]? OpenFiles(
            string? title = null,
            string? filter = null,
            string? initialDirectory = null);

        string? SaveFile(
            string? title = null,
            string? filter = null,
            string? initialDirectory = null,
            string? defaultFileName = null);

        string? SelectFolder(
            string? title = null,
            string? initialDirectory = null);
    }
}
