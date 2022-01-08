using Silver.Projects;

namespace Silver.Core
{
    public class Projects : Runtime
    {
        public static object? GetProperty(string filePath, string buildConfig, string prop)
        {
            var file = new FileInfo(ThrowIfFileNotFound(filePath));
            if(file.Extension == ".csproj")
            {
                var proj = new MSBuildSpecSharpProject(filePath, buildConfig);
                return GetProp(proj, prop);
            }
            else if (file.Extension == ".sscproj")
            {
                var proj = new XmlSpecSharpProject(filePath, buildConfig);
                return GetProp(proj, prop);
            }
            else
            {
                Error("The file {0} has an unrecognized extension. Valid extensions for Spec# projects are {1} and {2}.", filePath, ".csproj", ".sscproj");
                return null;
            }
        }

        public static string? GetCommandLine(string filePath, string buildConfig)
        {
            return SpecSharpProject.GetProject(ThrowIfFileNotFound(filePath), buildConfig)?.CommandLine;
        }

        public static void Compile(string file, string buildConfig)
        {
            FileInfo f = new FileInfo(file);

            SpecSharpProject project = f.Extension == ".csproj" ? new MSBuildSpecSharpProject(file, buildConfig) : new XmlSpecSharpProject(file, buildConfig);
        }
    }
}
