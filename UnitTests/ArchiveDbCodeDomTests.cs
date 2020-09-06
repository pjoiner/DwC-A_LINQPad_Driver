using DwC_A.Config;
using DwC_A_Driver;
using Xunit;

namespace UnitTests
{
    [Collection("MetaData Collection")]
    public class ArchiveDbCodeDomTests
    {
        readonly MetaDataFixture metaData;

        public ArchiveDbCodeDomTests(MetaDataFixture fixture)
        {
            metaData = fixture;
        }

        [Fact]
        public void ShouldCreateAchiveDbSourceFile()
        {
            var archiveDbCodDom = new ArchiveDbCodeDom();
            var code = archiveDbCodDom.GenerateSource(metaData.coreFileMetaData, 
                metaData.extensionFileMetaData);
            Assert.NotEmpty(code);
        }

        [Fact]
        public void ShouldCreateAchiveDbSourceFileWithOptions()
        {
            var archiveDbCodDom = new ArchiveDbCodeDom(false, 
                new FileReaderConfiguration() { BufferSize = 1024 },
                new RowFactoryConfiguration() { Strategy = RowStrategy.Greedy });
            var code = archiveDbCodDom.GenerateSource(metaData.coreFileMetaData,
                metaData.extensionFileMetaData);
            Assert.NotEmpty(code);
        }

    }
}
