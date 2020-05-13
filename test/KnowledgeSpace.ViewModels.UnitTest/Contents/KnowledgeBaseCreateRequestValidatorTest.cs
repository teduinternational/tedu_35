using KnowledgeSpace.ViewModels.Contents;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Contents
{
    public class KnowledgeBaseCreateRequestValidatorTest
    {
        private KnowledgeBaseCreateRequestValidator validator;
        private KnowledgeBaseCreateRequest request;

        public KnowledgeBaseCreateRequestValidatorTest()
        {
            request = new KnowledgeBaseCreateRequest()
            {
                CaptchaCode = "abc",
                Labels = new string[] { "test" },
                Environment = "test",
                SeoAlias = "test",
                Attachments = null,
                CategoryId = 1,
                Description = "test",
                ErrorMessage = "test",
                Note = "test",
                Problem = "test",
                StepToReproduce = "test",
                Title = "test",
                Workaround = "test"
            };
            validator = new KnowledgeBaseCreateRequestValidator();
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
        public void Should_Error_Result_When_Miss_Title(string title)
        {
            request.Title = title;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_CategoryId_Is_Zero()
        {
            request.CategoryId = 0;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Miss_Problem(string problem)
        {
            request.Problem = problem;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}