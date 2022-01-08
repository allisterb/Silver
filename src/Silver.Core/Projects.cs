using Silver.Projects;

namespace Silver.Core
{
    public class Projects : Runtime
    {
        public static object? GetProperty(string filePath, string prop)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            var file = new FileInfo(filePath);
            SpecSharpProject proj = file.Extension == ".csproj" ? new CsSpecSharpProject(filePath) : new XmlSpecSharpProject(filePath);
            return GetProp(proj, prop);
        }

        public static void Compile(string file)
        {
            FileInfo f = new FileInfo(file);

            SpecSharpProject project = f.Extension == ".csproj" ? new CsSpecSharpProject(file) : new XmlSpecSharpProject(file);
        }
    }
}
