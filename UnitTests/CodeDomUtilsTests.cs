using DwC_A_Driver;
using Xunit;

namespace UnitTests
{
    public class CodeDomUtilsTests
    {
        [Fact]
        public void ShouldPrependKeyWordWithAt()
        {
            var identifier = "event";
            var actual = CodeDomUtils.ModifyKeywords(identifier);
            Assert.Equal("@event", actual);
        }

        [Fact]
        public void ShouldCapitalizeKeyword()
        {
            var identifier = "event";
            var actual = CodeDomUtils.ModifyKeywords(identifier, true);
            Assert.Equal("Event", actual);
        }

        [Fact]
        public void ShouldAcceptValidIdentifier()
        {
            var identifier = "test";
            var actual = CodeDomUtils.ModifyKeywords(identifier);
            Assert.Equal("test", actual);
        }
    }
}
