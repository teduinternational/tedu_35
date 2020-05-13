using KnowledgeSpace.ViewModels.Contents;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Contents
{
    public class CategoryCreateRequestValidatorTest
    {
        private CategoryCreateRequestValidator validator;
        private CategoryCreateRequest request;

        public CategoryCreateRequestValidatorTest()
        {
            request = new CategoryCreateRequest()
            {
                Name = "test",
                ParentId = null,
                SeoAlias = "test",
                SeoDescription = "test",
                SortOrder = 1
            };
            validator = new CategoryCreateRequestValidator();
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

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Miss_SeoAlias(string seoAlias)
        {
            request.SeoAlias = seoAlias;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}