using KnowledgeSpace.BackendServer.Helpers;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Helpers
{
    public class TextHelperTest
    {
        [Fact]
        public void ToUnsignString_ValidInput_SuccessResult()
        {
            var result = TextHelper.ToUnsignString("tôi muốn chuyển sang không dấu");
            Assert.Equal("toi-muon-chuyen-sang-khong-dau", result);
        }
    }
}