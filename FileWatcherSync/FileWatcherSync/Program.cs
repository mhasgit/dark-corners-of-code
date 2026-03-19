using System.Diagnostics;
using System.Drawing;

namespace FileWatcherSync;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== File Watcher / Sync Tool ===\n");

        string sourceFolder;
        string backupFolder;

        if (args.Length >= 2)
        {
            sourceFolder = args[0];
            backupFolder = args[1];
        }
        else
        {
            Console.Write("Enter source folder to watch: ");
            sourceFolder = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(sourceFolder) || !Directory.Exists(sourceFolder))
            {
                Console.WriteLine("Invalid source folder");
                return;
            }

            Console.Write("Enter backup folder: ");
            backupFolder = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(backupFolder))
            {
                Console.WriteLine("Invalid backup folder");
                return;
            }
        }

        var watch = new FileSyncWatcher(sourceFolder, backupFolder);

        try
        {
            watch.Start();

            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
            } while (key.KeyChar != 'q' && key.KeyChar != 'Q');
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }
        finally
        {
            watch.Stop();
            Console.WriteLine("\n Stopped Watching");
        }
    }
}

public class FileSyncWatcher
{
    private readonly string _sourceFolder;
    private readonly string _backupFolder;
    private FileSystemWatcher? _watcher;


    public FileSyncWatcher(string sourceFolder, string destFolder)
    {
        _sourceFolder = Path.GetFullPath(sourceFolder) ?? throw new ArgumentNullException(nameof(sourceFolder));
        _backupFolder = Path.GetFullPath(destFolder) ?? throw new ArgumentNullException(nameof(destFolder));

        if (!Directory.Exists(_sourceFolder))
        {
            throw new DirectoryNotFoundException($"Source folder not found: {_sourceFolder}");
        }
    }

    public void Start()
    {
        Directory.CreateDirectory(_backupFolder);

        _watcher = new FileSystemWatcher
        {
            Path = _sourceFolder,
            Filter = "*.*",
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
        };

        _watcher.Created += OnFileCreated;
        _watcher.Changed += OnFileChanged;
        _watcher.Error += OnError;

        _watcher.EnableRaisingEvents = true;

        Console.WriteLine($"Watching: {_sourceFolder}");
        Console.WriteLine($"Backup to: {_backupFolder}");
        Console.WriteLine($"Prss 'q' to quit\n");

    }

    public void Stop()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= OnFileChanged;
            _watcher.Changed -= OnFileChanged;
            _watcher.Error -= OnError;
            _watcher.Dispose();
            _watcher = null;
        }
    }

    private void OnFileCreated(Object sender, FileSystemEventArgs e)
    {
        Task.Delay(1000).ContinueWith(_ => ProcessFile(e.FullPath, "Created"));
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (File.Exists(e.FullPath))
        {
            Task.Delay(1000).ContinueWith(_ => ProcessFile(e.FullPath, "Changed"));
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        Console.WriteLine($"Watcher error: {exception?.Message}");
    }

    private void ProcessFile(string sourcePath, string eventType)
    {
        try
        {
            if (Directory.Exists(sourcePath))
            {
                return;
            }

            string relativePath = Path.GetRelativePath(_sourceFolder, sourcePath);
            string destPath = Path.Combine(_backupFolder, relativePath);

            CopyFile(sourcePath, destPath);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {eventType}: {relativePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Processing {sourcePath}: {ex.Message}");
        }
    }

    private void CopyFile(string sourcePath, string destPath)
    {
        try
        {
            string? destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrWhiteSpace(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Copy(sourcePath, destPath, overwrite: true);
        }
        catch (UnauthorizedAccessException)
        {
            throw new Exception("Access denied. check file permission");
        }
        catch (DirectoryNotFoundException)
        {
            throw new Exception("Destination directory not found");
        }
        catch (IOException ex)
        {
            Thread.Sleep(200);
            try
            {
                File.Copy(sourcePath, destPath, overwrite: true);
            }
            catch
            {
                throw new Exception($"File is locked or in use: {ex.Message}");
            }
        }
    }
}
