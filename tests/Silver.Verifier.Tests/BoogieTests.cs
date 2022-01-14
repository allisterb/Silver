using Xunit;

using System.IO;
using System.Xml.Serialization;
using System.Xml;


namespace Silver.Verifier.Tests
{
    public class BoogieTests
    {
        [Fact]
        public void CanDeserializeResultsXml()
        {
            var path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "BoogieResults1.xml");
            Assert.True(File.Exists(path));
            XmlSerializer ser = new XmlSerializer(typeof(Models.BoogieResult));
            using XmlReader reader = XmlReader.Create(path);
            var results = (Models.BoogieResult?) ser.Deserialize(reader);
            Assert.NotNull(results);   
        }
    }
}