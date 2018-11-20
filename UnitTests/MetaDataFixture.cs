using DwC_A.Factories;
using DwC_A.Meta;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public class MetaDataFixture
    {
        IAbstractFactory factory = new DefaultFactory();
        readonly string path = "./resources";
        public readonly IFileMetaData coreFileMetaData;
        public readonly IEnumerable<IFileMetaData> extensionFileMetaData;

        public MetaDataFixture()
        {
            var metaDataReader = factory.CreateMetaDataReader();
            var metaData = metaDataReader.ReadMetaData(path);
            coreFileMetaData = factory.CreateCoreMetaData(metaData.Core);
            extensionFileMetaData = metaData.Extension.ToList()
                .Select(e => factory.CreateExtensionMetaData(e));
        }
    }
}
