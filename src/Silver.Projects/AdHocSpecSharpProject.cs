using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Projects
{
    public class AdHocSpecSharpProject : SpecSharpProject
    {
        public AdHocSpecSharpProject(Dictionary<string, object> settings) : base(settings.FailIfKeyNotPresent<IEnumerable<string>> ("SourceFiles").First(), settings.FailIfKeyNotPresent<string>("BuildConfig"))
        {
            BuildConfiguration = settings.TryGet<string>("BuildConfiguration") ?? "Debug";
            SourceFiles = (List<string>) settings["SourceFiles"];
            AssemblyName = settings.TryGet<string>("AssemblyName") ?? Path.GetFileNameWithoutExtension(SourceFiles.First());
            OutputType = settings.TryGet<string>("OutputType") ?? "library";
            TargetExt = OutputType.ToLower() == "exe" ? ".exe" : ".dll";
            TargetPath = settings.TryGet<string>("TargetPath") ?? Path.Combine(Path.GetDirectoryName(SourceFiles.First())!, AssemblyName) + TargetExt;
            RootNamespace = settings.TryGet<string>("RootNamespace");
            StartupObject = settings.TryGet<string>("StartupObject") ?? "library";
            StandardLibraryLocation = settings.TryGet<string>("StandardLibraryLocation") ?? "";
            TargetPlatformLocation = settings.TryGet<string>("TargetPlatformLocation") ?? "";
            ShadowedAssembly = settings.TryGet<string>("ShadowedAssembly") ?? "";
            Initialized = true;
        }
    }
}
