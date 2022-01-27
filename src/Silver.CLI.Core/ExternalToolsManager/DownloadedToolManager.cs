namespace Silver
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Runtime.InteropServices;

    internal class DownloadedToolManager : ToolManager
    {
        #region Constructors
        internal DownloadedToolManager(ToolSourceSettings settings) : base(settings)
        {
            
        }
        #endregion

        #region Properties
        protected string DownloadURL
        {
            get
            {
                return this.settings.DownloadURLs[OsName];
            }
        }

        protected string ExePathWithinZip
        {
            get
            {
                return this.settings.ExePathsWithinZip[OsName];
            }
        }

        protected string ZipFileName
        {
            get
            {
                return this.settings.Name + ".zip";
            }
        }

        protected string TempDirectory
        {
            get
            {
                return Path.Combine(this.settings.CommandPath, "Temp");
            }
        }

        protected string ZipFilePath
        {
            get
            {
                return Path.Combine(this.TempDirectory, this.ZipFileName);
            }
        }

        protected string ExtractPath
        {
            get
            {
                return Path.Combine(this.TempDirectory, this.settings.Name);
            }
        }

        protected string ExeTempPath
        {
            get
            {
                return Path.Combine(this.ExtractPath, this.ExePathWithinZip);
            }
        }
        #endregion

        #region Overriden members
        internal override void EnsureExists()
        {
            EnsureCommandPathExists();

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
        #endregion

        #region Methods
        protected virtual void ChangePermission()
        {
            if (OsName == "linux" || OsName == "osx")
            {
                RunCmd("chmod", $"+x {Command}", checkExists:false);
            }
        }

        protected virtual void Install()
        {
            DownloadAndUnZip();
        }

        protected virtual void DownloadAndUnZip()
        {
            using (var op = Begin("Download and unzip {0} to {1}.", this.settings.Name, ExtractPath))
            {
                PrepareTempDirectory();
                Download();
                ZipFile.ExtractToDirectory(ZipFilePath, ExtractPath);
                CopyToCommand(ExeTempPath);
                File.Delete(ZipFilePath);
                Directory.Delete(ExtractPath, true);
                op.Complete();
            }
        }

        protected virtual void DownloadAndCopy()
        {
            PrepareTempDirectory();
            Download();
            CopyToCommand(ZipFilePath);
            File.Delete(ZipFilePath);
        }

        protected virtual void CreateSymbolicLink()
        {
            if (OsName == "osx")
            {
                RunCmd("ln", $"-s {DownloadURL} {Command}");
            }
        }

        protected void PrepareTempDirectory()
        {
            if (!Directory.Exists(TempDirectory))
            {
                Debug("Creating temporary directory {0}.", TempDirectory);
                Directory.CreateDirectory(TempDirectory);
            }
            else
            {
                if (File.Exists(ZipFilePath))
                {
                    Debug("Deleting file {0}.", ZipFilePath);
                    File.Delete(ZipFilePath);
                }

                if (Directory.Exists(ExtractPath))
                {
                    Debug("Deleting directory {0}.", ExtractPath);
                    Directory.Delete(ExtractPath, true);
                }
            }
        }

        protected virtual void CopyToCommand(string exeTempPath)
        {
            Debug("Copying {0} to {1}.", exeTempPath, Command);
            File.Copy(exeTempPath, Command);
        }

        protected virtual void Download()
        {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            //using (var op = Begin("Downloading {0} from {1} to {2}", this.settings.Name, DownloadURL, TempDirectory))
            //using (var client = new WebClient())
            //{
            //    client.DownloadFile(DownloadURL, ZipFilePath);
            //    op.Complete();
            //}
            DownloadFile(this.settings.Name, new Uri(DownloadURL), ZipFilePath);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        }

        internal void EnsureLinkedToZ3(ToolManager z3)
        {
            var z3DependencyPath = GetZ3DependencyPath(z3);

            // Workaround: Boogie and Corral are looking for z3.exe, even on linux/mac
            if (!z3DependencyPath.EndsWith(".exe"))
            {
                z3DependencyPath += ".exe";
            }

            if (!File.Exists(z3DependencyPath))
            {
                Info("Copying {0} to {1}.", z3.Command, z3DependencyPath);
                File.Copy(z3.Command, z3DependencyPath);
            }
            else
            {
                Info("Dependency {0} exists at {1}.", "z3", z3DependencyPath);
            }
        }

        protected string GetZ3DependencyPath(ToolManager z3)
        {
            return Path.Combine(this.DependencyTargetPath.Replace("/", PathSeparator), z3.ExeName);

        }

        internal string DependencyTargetPath
        {
            get => Path.Combine(this.settings.CommandPath, this.settings.DependencyRelativePath);
        }
        #endregion
    }
}
