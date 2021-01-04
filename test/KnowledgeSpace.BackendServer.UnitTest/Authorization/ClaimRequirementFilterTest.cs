using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Authorization
{
    public class ClaimRequirementFilterTest
    {
        [Fact]
        public void Constructor_CreateInstance_ShouldBe_NotNull()
        {
            var obj = new ClaimRequirementFilter(FunctionCode.CONTENT, CommandCode.CREATE);
            Assert.NotNull(obj);
        }

        [Fact]
        public void OnAuthorization_ValidClaims_SuccessResult()
        {
            var obj = new ClaimRequirementFilter(FunctionCode.CONTENT, CommandCode.CREATE);
            var permissions = new List<string>() { "CONTENT_CREATE" };
            var claims = new List<Claim>() {
                new Claim(SystemConstants.Claims.Permissions,JsonSerializer.Serialize(permissions))
            };
            var modelState = new ModelStateDictionary();

            var httpContextMock = new DefaultHttpContext();

            httpContextMock.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { }); // if you are reading any properties from the query parameters
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextMock.User = claimsPrincipal;

            var actionContext = new ActionContext(
                httpContextMock,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new AuthorizationFilterContext(
                actionContext,
                new List<IFilterMetadata>())
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };

            obj.OnAuthorization(actionExecutingContext);

            Assert.IsType<OkResult>(actionExecutingContext.Result);
        }
    }
}