using Silver.Projects;

namespace Silver.Core
{
    public class Projects : Runtime
    {
        public static object? GetProperty(string filePath, string buildConfig, string prop)
        {
            var file = new FileInfo(FailIfFileNotFound(filePath));
            if(file.Extension == ".csproj")
            {
                var proj = new MSBuildSpecSharpProject(filePath, buildConfig);
                return proj.Initialized ? GetProp(proj, prop) : null;
            }
            else if (file.Extension == ".sscproj")
            {
                var proj = new XmlSpecSharpProject(filePath, buildConfig);
                return proj.Initialized ? GetProp(proj, prop) : null;
            }
            else
            {
                Error("The file {0} has an unrecognized extension. Valid extensions for Spec# projects are {1} and {2}.", filePath, ".csproj", ".sscproj");
                return null;
            }
        }

        public static string? GetCommandLine(string filePath, string buildConfig)
        {
            return SpecSharpProject.GetProject(FailIfFileNotFound(filePath), buildConfig)?.CommandLine;
        }

        public static bool Compile(string filePath, string buildConfig)
        {
            var proj = SpecSharpProject.GetProject(FailIfFileNotFound(filePath), buildConfig);
            if (proj is not null && proj.Initialized)
            {
                return proj.Compile();
            }
            else
            {
                return false;
            }
        }
    }
}
