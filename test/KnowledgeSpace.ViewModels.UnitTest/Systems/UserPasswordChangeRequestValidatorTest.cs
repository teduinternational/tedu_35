using KnowledgeSpace.ViewModels.Systems;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Systems
{
    public class UserPasswordChangeRequestValidatorTest
    {
        private UserPasswordChangeRequestValidator validator;
        private UserPasswordChangeRequest request;

        public UserPasswordChangeRequestValidatorTest()
        {
            request = new UserPasswordChangeRequest()
            {
                CurrentPassword = "Test@123$",
                NewPassword = "Test@123$1",
                UserId = "1"
            };
            validator = new UserPasswordChangeRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_UserId()
        {
            request.UserId = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Request_Miss_CurrentPassword(string data)
        {
            request.CurrentPassword = data;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Request_Miss_NewPassword(string data)
        {
            request.NewPassword = data;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_NewPassword_Is_Not_Enough_8_Characters()
        {
            request.NewPassword = "Test@1$";
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_NewPassword_Is_Not_Complex()
        {
            request.NewPassword = "Test";
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_NewPassword_Is_Same_OldPassword()
        {
            request.NewPassword = request.CurrentPassword;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}