namespace Silver.Verifier;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Silver.Verifier.Models;
public class Boogie : Runtime
{
    public static string? Translate(string path, string? classname = null, string? methodname = null)
    {
        using var op = Begin("Dissassembling assembly {0} to Boogie", path);
        var f = DateTime.Now.Ticks.ToString() + Path.GetFileName(path) + ".boogie";
        string filter = "";
        if (classname is not  null && methodname is null)
        {
            filter = $" /trClass:{classname}";
        }
        else if (classname is null && methodname is not null)
        {
            filter = $" /trMethod:{classname}";
        }
        else if (classname is not null && methodname is not null)
        {
            filter = $" /trMethod:{classname + "." + methodname}";
        }
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie.exe"), path + filter + " /print:" + f + " /noVerify", isNETFxTool: true);

        if (ret is null)
        {
            Error("Could not run Boogie translator.");
            File.Delete(f);
            op.Abandon();
            return null;
        }
        else if (!ret.Contains("Spec# program verifier finished"))
        {
            Error(ret);
            Error("Boogie translator did not complete successfully.");
            File.Delete(f);
            op.Abandon();
            return null;
        }
        else
        {
            op.Complete();
            var b = File.ReadAllText(f);
            File.Delete(f);
            return b;
        }
    }

    public static BoogieResults? Verify(string path, string? output = null)
    {
        var f = output ?? DateTime.Now.Ticks.ToString() + Path.GetFileName(FailIfFileNotFound(path)) + ".xml";
        Debug("XML output will be written to file {0}.", Path.GetFullPath(f));
        WarnIfFileExists(f);
        var op = Begin("Verifying {0}", path);
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie.exe"), path + " " + "/xml:" + f, isNETFxTool: true);
        if (ret is null)
        {
            Error("Could not run program verifier.");
            op.Abandon();
            File.Delete(f);
            return null;
        }
        else if (!ret.Contains("Spec# program verifier finished"))
        {
            Error("Program verifier did not complete successfully");
            op.Abandon();
            if (output is null) File.Delete(f);
            return null;
        }
        else
        {
            XmlSerializer ser = new XmlSerializer(typeof(BoogieResults));
            using XmlReader reader = XmlReader.Create(f);
            var results = (BoogieResults?)ser.Deserialize(reader);
            reader.Close();
            reader.Dispose();
            if (output is null)
            {
                Debug("Deleting file {0}.", Path.GetFullPath(f));
                File.Delete(f);
            }
            op.Complete();
            return results;
        }
    }
}

