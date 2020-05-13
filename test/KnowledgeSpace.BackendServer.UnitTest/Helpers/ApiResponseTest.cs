using KnowledgeSpace.BackendServer.Helpers;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Helpers
{
    public class ApiResponseTest
    {
        [Fact]
        public void Constructor_CreateInstance_ShouldBe_NotNull()
        {
            var obj = new ApiResponse(400, "error");
            Assert.NotNull(obj);
            Assert.Equal(400, obj.StatusCode);
        }

        [Fact]
        public void GetDefaultMessageForStatusCode_ValidStatusCode_ReturnCorrectMessage()
        {
            var obj = new ApiResponse(404);
            Assert.Equal("Resource not found", obj.Message);
        }
    }
}