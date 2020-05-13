using KnowledgeSpace.ViewModels.Contents;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Contents
{
    public class LabelCreateRequestValidatorTest
    {
        private LabelCreateRequestValidator validator;
        private LabelCreateRequest request;

        public LabelCreateRequestValidatorTest()
        {
            request = new LabelCreateRequest()
            {
                Name = "test"
            };
            validator = new LabelCreateRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Miss_Name(string name)
        {
            request.Name = name;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}