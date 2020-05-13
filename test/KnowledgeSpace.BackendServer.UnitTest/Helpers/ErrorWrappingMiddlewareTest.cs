using KnowledgeSpace.BackendServer.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Helpers
{
    public class ErrorWrappingMiddlewareTest
    {
        [Fact]
        public async Task ErrorWrapping_Middleware_Should_Return_Error()
        {
            var exceptionHandler = new ErrorWrappingMiddleware((innerHttpContext) =>
            {
                throw new Exception("Test exception");
            }, new NullLogger<ErrorWrappingMiddleware>());

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream(); // <== Replace the NullStream

            await exceptionHandler.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var text = reader.ReadToEnd();
            Assert.False(string.IsNullOrEmpty(text));
        }
    }
}