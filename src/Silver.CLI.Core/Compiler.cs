using Silver.Projects;
using Stratis.SmartContracts.CLR.Compilation;

namespace Silver.CLI.Core
{
    public class Compiler : Runtime
    {
       public static bool PrintProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
        {
            var p = SilverProject.GetProperty(FailIfFileNotFound(filePath), buildConfig, prop, additionalFiles);
            if (p is null)
            {
                Error("The property {0} does not exist or is null for the project file {1}.", prop, filePath);
                return false;
            }
            else
            {
                Info("The compile-time value of property {0} in build configuration {1} is {2}.", prop, buildConfig, p);
                return true;
            }
        }

        public static bool GetCommandLine(string filePath, string buildConfig, params string [] additionalFiles)
        {
            var l = SilverProject.GetProject(FailIfFileNotFound(filePath), buildConfig, additionalFiles)?.CommandLine;
            {
                if (l is null)
                {
                    Error("Could not get Spec# command line for project file {0}.", filePath);
                    return false;
                }
                else
                {
                    Info("Spec# compiler command-line is {0}.", "ssc " + l);
                    return true;
                }
            }
        }

        public static bool Compile(string filePath, string buildConfig, bool verify = false, params string[] additionalFiles)
        {
            var proj = SilverProject.GetProject(FailIfFileNotFound(filePath), buildConfig, additionalFiles);
            if (proj is not null && proj.Initialized)
            {
                proj.Verify = verify;
                return proj.Compile();
            }
            else
            {
                return false;
            }
        }
    }
}
