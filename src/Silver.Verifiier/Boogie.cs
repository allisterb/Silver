namespace Silver.Verifier;

using System.Xml;
using System.Xml.Serialization;

using Silver.Verifier.Models;
public class Boogie : Runtime
{
    public static BoogieResults? Verify(string path)
    {
        var f = DateTime.Now.Ticks.ToString() + Path.GetFileName(FailIfFileNotFound(path)) + ".xml";
        Debug("XML output will be written to file {0}.", Path.GetFullPath(f));
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie.exe"), path + " " + "/xml:"+ f, isNETFxTool: true);
        if (ret is null)
        {
            Error("Coould not run program verifier.");
            return null;
        }
        else if(!ret.Contains("Spec# program verifier finished"))
        {
            Error("Program verifier did not complete successfully");
            return null;
        }
        else
        {
            XmlSerializer ser = new XmlSerializer(typeof(BoogieResults));
            using XmlReader reader = XmlReader.Create(f);
            var results =  (BoogieResults?)ser.Deserialize(reader);
            reader.Close();
            reader.Dispose();
            Debug("Deleting file {0}.", Path.GetFullPath(f));
            File.Delete(f);
            return results;
        }
    }
}

