using DwC_A_Driver;
using Xunit;

namespace UnitTests
{
    [Collection("MetaData Collection")]
    public class ArchiveFileCodeDomTests
    {
        readonly MetaDataFixture metaData;

        public ArchiveFileCodeDomTests(MetaDataFixture fixture)
        {
            metaData = fixture;
        }

        [Fact]
        public void ShouldBuildCoreFileClass()
        {
            var archiveFileCodeDom = new ArchiveFileCodeDom();
            var fileName = "taxon.txt";
            var code = archiveFileCodeDom.GenerateSource(fileName, metaData.coreFileMetaData);
            Assert.NotEmpty(code);
        }
    }
}
