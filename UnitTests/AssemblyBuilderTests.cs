using Xunit;
using DwC_A_Driver;
using System.IO;

namespace UnitTests
{
    public class AssemblyBuilderTests
    {
        const string fileName = "./resources/dwca-vascan-v37.5.zip";

        [Fact]
        public void ShouldBuildAssembly()
        {
            File.Delete("test.dll");
            var archiveDbAssemblyBuilder = new ArchiveDbAssemblyBuilder();
            archiveDbAssemblyBuilder.GenerateArchiveDbAssembly(fileName, "test.dll", ".");
            Assert.True(File.Exists("test.dll"));
        }
    }
}
