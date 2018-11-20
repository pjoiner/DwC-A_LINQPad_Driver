using Xunit;
using DwC_A_Driver;
using System.IO;
using System.Linq;

namespace UnitTests
{
    [Collection("MetaData Collection")]
    public class AssemblyBuilderTests
    {
        readonly MetaDataFixture metaData;

        public AssemblyBuilderTests(MetaDataFixture fixture)
        {
            metaData = fixture;
        }

        [Fact]
        public void ShouldBuildAssembly()
        {
            var archiveDbAssemblyBuilder = new ArchiveDbAssemblyBuilder();
            archiveDbAssemblyBuilder.GenerateArchiveDbAssembly(metaData.coreFileMetaData, 
                metaData.extensionFileMetaData, 
                "test.dll", ".");
            Assert.True(File.Exists("test.dll"));
            Directory.GetFiles(".", "test.*").ToList().ForEach(File.Delete);
        }
    }
}
