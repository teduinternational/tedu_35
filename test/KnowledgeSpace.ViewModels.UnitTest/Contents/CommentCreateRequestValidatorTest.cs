using KnowledgeSpace.ViewModels.Contents;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Contents
{
    public class CommentCreateRequestValidatorTest
    {
        private CommentCreateRequestValidator validator;
        private CommentCreateRequest request;

        public CommentCreateRequestValidatorTest()
        {
            request = new CommentCreateRequest()
            {
                CaptchaCode = "abc",
                Content = "test",
                KnowledgeBaseId = 1,
                ReplyId = null
            };
            validator = new CommentCreateRequestValidator();
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
        public void Should_Error_Result_When_Miss_Content(string content)
        {
            request.Content = content;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Miss_Captcha(string captcha)
        {
            request.CaptchaCode = captcha;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_KnowledgeBaseId_Is_Zero()
        {
            request.KnowledgeBaseId = 0;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}