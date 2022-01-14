namespace Silver.Verifiier;

using System.Xml;
using System.Xml.Serialization;

using Silver.Verifier.Models;
public class Boogie
{
    public static BoogieResults? Verify(string path)
    {
        XmlSerializer ser = new XmlSerializer(typeof(BoogieResults));
        using XmlReader reader = XmlReader.Create(path);
        return (BoogieResults?) ser.Deserialize(reader);
    }
}

