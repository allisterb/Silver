namespace Silver;

using System.IO;
using System.IO.Compression;

internal class DownloadedDotNetToolManager : DownloadedToolManager
{
    #region Constructors
    internal DownloadedDotNetToolManager(ToolSourceSettings settings, IInterface managerInterface) : base(settings, managerInterface)
    {
        this.Directory = settings.Directory;
    }
    #endregion

    #region Overriden members
    public override string Command => Path.Combine(this.settings.CommandPath, this.Directory, this.ExeName);

    public override string ExeName => this.settings.Name + ".exe";

    internal override void EnsureExists()
    {
        EnsureDirectoryExists();

        if (!Exists())
        {
            Install();
            ChangePermission();
        }
        else
        {
            Info("{0} exists in {1}.", this.settings.Name, this.settings.CommandPath);
        }
    }

    protected override void DownloadAndUnZip()
    {
        using (var op = Begin("Download and unzip {0} to {1}", this.settings.Name, Path.Combine(AssemblyLocation, Directory)))
        {
            PrepareTempDirectory();
            Download();
            ZipFile.ExtractToDirectory(ZipFilePath, ExtractPath);
            CopyDirectory(ExtractPath, Path.Combine(AssemblyLocation, Directory));
            File.Delete(ZipFilePath);
            System.IO.Directory.Delete(ExtractPath, true);
            op.Complete();
        }
    }
    #endregion

    #region Properties
    public string Directory { get; }
    #endregion

    #region Methods
    protected virtual void EnsureDirectoryExists()
    {
        if (!System.IO.Directory.Exists(this.Directory))
        {
            System.IO.Directory.CreateDirectory(this.Directory);
        }
    }
    #endregion
}

