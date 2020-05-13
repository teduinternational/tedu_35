using KnowledgeSpace.BackendServer.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Helpers
{
    public class ApiBadRequestResponseTest
    {
        [Fact]
        public void Constructor_CreateInstance_ShouldBe_NotNull()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("", "test error");
            var obj = new ApiBadRequestResponse(modelState);
            Assert.NotNull(obj);
            Assert.NotNull(obj.Errors);
        }

        [Fact]
        public void Constructor_CreateInstance_ModelState_Valid_ShouldBe_Throw_Exception()
        {
            var modelState = new ModelStateDictionary();
            Assert.ThrowsAny<ArgumentException>(() => new ApiBadRequestResponse(modelState));
        }

        [Fact]
        public void Constructor_CreateInstance_Identity_Result_NotNul()
        {
            var obj = new ApiBadRequestResponse(IdentityResult.Failed(new IdentityError[]{
                new IdentityError(){Code="",Description=""}
            }));
            Assert.NotNull(obj);
            Assert.Equal(400, obj.StatusCode);
        }

        [Fact]
        public void Constructor_CreateInstance_Message_NotNul()
        {
            var obj = new ApiBadRequestResponse("error");
            Assert.NotNull(obj);
            Assert.Equal("error", obj.Message);
        }
    }
}