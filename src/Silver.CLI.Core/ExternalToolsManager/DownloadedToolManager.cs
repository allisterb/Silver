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
        private string DownloadURL
        {
            get
            {
                return this.settings.DownloadURLs[OsName];
            }
        }

        private string ExePathWithinZip
        {
            get
            {
                return this.settings.ExePathsWithinZip[OsName];
            }
        }

        private string ZipFileName
        {
            get
            {
                return this.settings.Name + ".zip";
            }
        }

        private string TempDirectory
        {
            get
            {
                return Path.Combine(this.settings.CommandPath, "Temp");
            }
        }

        private string ZipFilePath
        {
            get
            {
                return Path.Combine(this.TempDirectory, this.ZipFileName);
            }
        }

        private string ExtractPath
        {
            get
            {
                return Path.Combine(this.TempDirectory, this.settings.Name);
            }
        }

        private string ExeTempPath
        {
            get
            {
                return Path.Combine(this.ExtractPath, this.ExePathWithinZip);
            }
        }

        internal DownloadedToolManager(ToolSourceSettings settings) : base(settings)
        {
        }

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

        private void ChangePermission()
        {
            if (OsName == "linux" || OsName == "osx")
            {
                RunCmd("chmod", $"+x {Command}");
            }
        }

        

        protected virtual void Install()
        {
            DownloadAndUnZip();
        }

        protected void DownloadAndUnZip()
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

        protected void DownloadAndCopy()
        {
            PrepareTempDirectory();
            Download();
            CopyToCommand(ZipFilePath);
            File.Delete(ZipFilePath);
        }

        protected void CreateSymbolicLink()
        {
            if (OsName == "osx")
            {
                RunCmd("ln", $"-s {DownloadURL} {Command}");
            }
        }

        private void PrepareTempDirectory()
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

        private void CopyToCommand(string exeTempPath)
        {
            Debug("Copying {0} to {1}.", exeTempPath, Command);
            File.Copy(exeTempPath, Command);
        }

        private void Download()
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
    }
}
