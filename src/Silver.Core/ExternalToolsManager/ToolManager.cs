namespace Silver;

using System.IO;
using System.Runtime.InteropServices;


public abstract class ToolManager : Runtime
{
    #region Constructors
    internal ToolManager(ToolSourceSettings settings) : base()
    {
        this.settings = settings;
    }
    #endregion

    #region Properties
    protected static string OsName
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx";
            }
            else
            {
                return "linux";
            }
        }
    }
    protected readonly ToolSourceSettings settings;

    public string ExeName
    {
        get
        {
            if (OsName == "windows")
            {
                return this.settings.Name + ".exe";
            }
            else
            {
                return this.settings.Name;
            }
        }
    }

    public string Command
    {
        get
        {
            return Path.Combine(this.settings.CommandPath, this.ExeName);
        }
    }
    #endregion

    #region Methods
    internal abstract void EnsureExists();

    protected bool Exists()
    {
        return File.Exists(this.Command);
    }

    protected void EnsureCommandPathExists()
    {
        if (!Directory.Exists(this.settings.CommandPath))
        {
            Directory.CreateDirectory(this.settings.CommandPath);
        }
    }
    #endregion
}

