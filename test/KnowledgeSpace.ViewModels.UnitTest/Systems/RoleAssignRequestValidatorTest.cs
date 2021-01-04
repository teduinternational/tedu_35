using KnowledgeSpace.ViewModels.Systems;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Systems
{
    public class RoleAssignRequestValidatorTest
    {
        private RoleAssignRequestValidator validator;
        private RoleAssignRequest request;

        public RoleAssignRequestValidatorTest()
        {
            request = new RoleAssignRequest()
            {
                RoleNames = new string[] { "Admin" }
            };
            validator = new RoleAssignRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_RoleNames()
        {
            request.RoleNames = null;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_RoleNames_Is_Empty()
        {
            request.RoleNames = new string[] { };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_RoleNames_Contains_Empty_Item()
        {
            request.RoleNames = new string[] { "" };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}