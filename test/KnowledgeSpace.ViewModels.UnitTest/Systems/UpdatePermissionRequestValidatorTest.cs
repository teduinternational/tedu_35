using KnowledgeSpace.ViewModels.Systems;
using System.Collections.Generic;
using Xunit;

namespace KnowledgeSpace.ViewModels.UnitTest.Systems
{
    public class UpdatePermissionRequestValidatorTest
    {
        private UpdatePermissionRequestValidator validator;
        private UpdatePermissionRequest request;

        public UpdatePermissionRequestValidatorTest()
        {
            request = new UpdatePermissionRequest()
            {
                Permissions = new List<PermissionVm>()
                {
                    new PermissionVm()
                        {
                            CommandId ="ADD",
                            FunctionId = "CONTENT",
                            RoleId = "Admin"
                        }
                }
            };
            validator = new UpdatePermissionRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_Permissions()
        {
            request.Permissions = null;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Permissions_Is_Empty()
        {
            request.Permissions = new List<PermissionVm>();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Request_Permissions_Contains_Miss_CommanndId_Item(string data)
        {
            request.Permissions = new List<PermissionVm>() {
                new PermissionVm()
                {
                    CommandId = data
                }
            };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Request_Permissions_Contains_Miss_FunctionId_Item(string data)
        {
            request.Permissions = new List<PermissionVm>() {
                new PermissionVm()
                {
                    FunctionId = data
                }
            };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Request_Permissions_Contains_Miss_RoleId_Item(string data)
        {
            request.Permissions = new List<PermissionVm>() {
                new PermissionVm()
                {
                    RoleId = data
                }
            };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}