using KnowledgeSpace.BackendServer.Controllers;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Services;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Controllers
{
    public class VotesControllerTest
    {
        private ApplicationDbContext _context;
        private Mock<ISequenceService> _mockSequenceService;
        private Mock<IStorageService> _mockStorageService;
        private Mock<ILogger<KnowledgeBasesController>> _mockLoggerService;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<IViewRenderService> _mockViewRenderService;
        private Mock<ICacheService> _mockCacheService;
        private Mock<IOneSignalService> _oneSignalService;
        public VotesControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext("VotesControllerTest");
            _mockSequenceService = new Mock<ISequenceService>();
            _mockStorageService = new Mock<IStorageService>();
            _mockLoggerService = new Mock<ILogger<KnowledgeBasesController>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockViewRenderService = new Mock<IViewRenderService>();
            _mockCacheService = new Mock<ICacheService>();
            _oneSignalService = new Mock<IOneSignalService>();
        }

        [Fact]
        public async Task GetVotes_ValidKbId_RecordMatch()
        {
            var controller = new KnowledgeBasesController(_context, _mockSequenceService.Object, _mockStorageService.Object,
                           _mockLoggerService.Object, _mockEmailSender.Object, _mockViewRenderService.Object, _mockCacheService.Object,_oneSignalService.Object);
            _context.Votes.AddRange(new List<Vote>()
            {
                new Vote(){ KnowledgeBaseId = 1, UserId = Guid.NewGuid().ToString(), CreateDate = DateTime.Now},
                new Vote(){ KnowledgeBaseId = 1, UserId = Guid.NewGuid().ToString(), CreateDate = DateTime.Now},
                new Vote(){ KnowledgeBaseId = 1, UserId = Guid.NewGuid().ToString(), CreateDate = DateTime.Now},
                new Vote(){ KnowledgeBaseId = 2, UserId = Guid.NewGuid().ToString(), CreateDate = DateTime.Now},
            });
            await _context.SaveChangesAsync();

            var result = await controller.GetVotes(1);

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            var votes = okResult.Value as List<VoteVm>;
            Assert.True(votes.Count == 3);
        }

        [Fact]
        public async Task PostVote_ValidInput_Success()
        {
            var controller = new KnowledgeBasesController(_context, _mockSequenceService.Object, _mockStorageService.Object,
                                     _mockLoggerService.Object, _mockEmailSender.Object, _mockViewRenderService.Object,
                                     _mockCacheService.Object, _oneSignalService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                }, "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _context.Votes.AddRange(new List<Vote>()
            {
                new Vote(){ KnowledgeBaseId = 1, UserId = Guid.NewGuid().ToString(), CreateDate = DateTime.Now},
            });
            _context.KnowledgeBases.AddRange(new List<KnowledgeBase>()
            {
                new KnowledgeBase(){ Id =1 , CategoryId =1, Title = "test",Problem = "test", Note = "test"},
            });

            await _context.SaveChangesAsync();
            var result = await controller.PostVote(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PostVote_NotFoundKbId_BadRequest()
        {
            var controller = new KnowledgeBasesController(_context, _mockSequenceService.Object, _mockStorageService.Object,
                                     _mockLoggerService.Object, _mockEmailSender.Object, _mockViewRenderService.Object,
                                     _mockCacheService.Object, _oneSignalService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                }, "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _context.Votes.AddRange(new List<Vote>()
            {
                new Vote(){ KnowledgeBaseId = 1, UserId = Guid.NewGuid().ToString(), CreateDate = DateTime.Now},
            });
            await _context.SaveChangesAsync();
            var result = await controller.PostVote(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}