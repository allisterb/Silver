using Silver.Projects;

namespace Silver.Core
{
    public class Projects : Runtime
    {
        public static object? GetProperty(string filePath, string prop)
        {
            var file = new FileInfo(ThrowIfFileNotFound(filePath));
            if(file.Extension == ".csproj")
            {
                var proj = new MSBuildSpecSharpProject(filePath);
                return GetProp(proj, prop);
            }
            else
            {
                var proj = new XmlSpecSharpProject(filePath);
                return GetProp(proj, prop);
            }
        }

        public static string? GetCommandLine(string filePath)
        {
            return SpecSharpProject.GetProject(ThrowIfFileNotFound(filePath))?.CommandLine;
        }

        public static void Compile(string file)
        {
            FileInfo f = new FileInfo(file);

            SpecSharpProject project = f.Extension == ".csproj" ? new MSBuildSpecSharpProject(file) : new XmlSpecSharpProject(file);
        }
    }
}
