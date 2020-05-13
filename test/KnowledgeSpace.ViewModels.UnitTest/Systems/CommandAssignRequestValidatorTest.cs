using KnowledgeSpace.ViewModels.Systems;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Systems
{
    public class CommandAssignRequestValidatorTest
    {
        private CommandAssignRequestValidator validator;
        private CommandAssignRequest request;

        public CommandAssignRequestValidatorTest()
        {
            request = new CommandAssignRequest()
            {
                AddToAllFunctions = true,
                CommandIds = new string[] { "ADD" }
            };
            validator = new CommandAssignRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_CommandIds()
        {
            request.CommandIds = null;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_CommandIds_Empty()
        {
            request.CommandIds = new string[] { };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_CommandIds_Contains_Any_Empty_Item()
        {
            request.CommandIds = new string[] { "" };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}