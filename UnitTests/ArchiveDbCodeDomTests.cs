using DwC_A;
using DwC_A_Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class ArchiveDbCodeDomTests
    {
        [Fact]
        public void ShouldCreateAchiveDbSourceFile()
        {
            var fileName = "./resources/dwca-vascan-v37.5.zip";
            using (var archive = new ArchiveReader(fileName))
            {
                var archiveDbCodDom = new ArchiveDbCodeDom();
                var code = archiveDbCodDom.GenerateSource(archive);
                Assert.NotEmpty(code);
            }
        }
    }
}
