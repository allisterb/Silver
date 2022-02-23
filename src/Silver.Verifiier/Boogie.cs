namespace Silver.Verifier;

using System.Xml;
using System.Xml.Serialization;

using Silver.Verifier.Models;
public class Boogie : Runtime
{
    public static BoogieResults? Verify(string path, string? output = null)
    {
        var f = output ?? DateTime.Now.Ticks.ToString() + Path.GetFileName(FailIfFileNotFound(path)) + ".xml";
        Debug("XML output will be written to file {0}.", Path.GetFullPath(f));
        WarnIfFileExists(f);
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie.exe"), path + " " + "/xml:"+ f, isNETFxTool: true);
        if (ret is null)
        {
            Error("Coould not run program verifier.");
            File.Delete(f);
            return null;
        }
        else if(!ret.Contains("Spec# program verifier finished"))
        {
            Error("Program verifier did not complete successfully");
            if (output is null) File.Delete(f);
            return null;
        }
        else
        {
            XmlSerializer ser = new XmlSerializer(typeof(BoogieResults));
            using XmlReader reader = XmlReader.Create(f);
            var results =  (BoogieResults?)ser.Deserialize(reader);
            reader.Close();
            reader.Dispose();
            if (output is null)
            {
                Debug("Deleting file {0}.", Path.GetFullPath(f));
                File.Delete(f);
            }
            return results;
        }
    }
}

