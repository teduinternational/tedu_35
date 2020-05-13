using KnowledgeSpace.ViewModels.Systems;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Systems
{
    public class RoleCreateRequestValidatorTest
    {
        private RoleCreateRequestValidator validator;
        private RoleCreateRequest request;

        public RoleCreateRequestValidatorTest()
        {
            request = new RoleCreateRequest()
            {
                Id = "admin",
                Name = "admin"
            };
            validator = new RoleCreateRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_RoleId()
        {
            request.Id = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_RoleName()
        {
            request.Name = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Role_Empty()
        {
            request.Id = string.Empty;
            request.Name = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}