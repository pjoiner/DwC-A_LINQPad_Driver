using DwC_A.Meta;
using DwC_A_Driver;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{
    public class ArchiveFileCodeDomTests
    {
        Mock<IFileMetaData> mockFileMetaData = new Mock<IFileMetaData>();
        Mock<IFieldMetaData> mockFieldMetaData = new Mock<IFieldMetaData>();
        
        public ArchiveFileCodeDomTests()
        {
            var id = new IdFieldType()
            {
                Index = 0,
                IndexSpecified = true
            };
            IList<FieldType> fields = new List<FieldType>()
            {
                new FieldType(){ Index = 0, Term = "Term1"},
                new FieldType(){ Index = 1, Term = "http://mydomain/Term2"}
            };
            mockFieldMetaData.Setup(n => n.GetEnumerator()).Returns(fields.GetEnumerator);

            mockFileMetaData.Setup(n => n.FileName).Returns("taxon.txt");
            mockFileMetaData.Setup(n => n.Id).Returns(id);
            mockFileMetaData.Setup(n => n.Fields).Returns(mockFieldMetaData.Object);
        }

        [Fact]
        public void ShouldBuildCoreFileClass()
        {
            var archiveFileCodeDom = new ArchiveFileCodeDom();
            var fileName = "taxon.txt";
            var code = archiveFileCodeDom.GenerateSource(fileName, mockFileMetaData.Object);
            Assert.NotEmpty(code);
        }
    }
}
