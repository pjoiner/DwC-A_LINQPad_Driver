using DwC_A_Driver;
using Xunit;

namespace UnitTests
{
    [Collection("MetaData Collection")]
    public class LINQPadSchemaGeneratorTests
    {
        readonly MetaDataFixture metaData;
        const string fileName = "whales.zip";

        public LINQPadSchemaGeneratorTests(MetaDataFixture fixture)
        {
            metaData = fixture;
        }

        [Fact]
        public void ShouldGenerateLINQPadSchema()
        {
            var linqPadSchemaGenerator = new LINQPadSchemaGenerator();
            var schema = linqPadSchemaGenerator.GenerateSchema(fileName, 
                metaData.coreFileMetaData,
                metaData.extensionFileMetaData);
            Assert.NotEmpty(schema);
        }
    }
}
