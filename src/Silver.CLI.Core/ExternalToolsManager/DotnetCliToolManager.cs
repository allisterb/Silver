namespace Silver
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class DotnetCliToolManager : ToolManager
    {
        #region Constructor
        internal DotnetCliToolManager(ToolSourceSettings settings) : base(settings)
        {
        }
        #endregion

        #region Methods
        internal string DependencyTargetPath
        {
            get => Path.Combine(this.settings.CommandPath, this.settings.DependencyRelativePath);
        }

        internal override void EnsureExists()
        {
            EnsureCommandPathExists();
            if (!Exists())
            {
                InstallDotnetCliTool();
            }
            else
            {
                Info(".NET CLI tool {0} exists in {1}.", this.settings.Name, this.settings.CommandPath);
            }
        }

        private string InstallDotnetCliTool()
        {
            using (var op = Runtime.Begin("Installing .NET tool {0} into {1}.", this.settings.Name, this.settings.CommandPath))
            using (Process p = new Process()) 
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "dotnet";
                p.StartInfo.Arguments = $"tool install {this.settings.Name} --tool-path {this.settings.CommandPath} --version {this.settings.Version}";
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                string errorMsg = p.StandardError.ReadToEnd();
                p.StandardOutput.Close();
                p.StandardError.Close();

                if (!String.IsNullOrEmpty(errorMsg))
                {
                    Error("Installation error: {0}", errorMsg);
                    op.Cancel();
                    return output;
                }
                else
                {
                    op.Complete();
                    return output;
                }
            }
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

        private string GetZ3DependencyPath(ToolManager z3)
        {
            return Path.Combine(this.DependencyTargetPath.Replace("/", PathSeparator), z3.ExeName);

        }
        #endregion
    }
}
