using KnowledgeSpace.BackendServer.Controllers;
using KnowledgeSpace.BackendServer.Data;
using Xunit;

namespace KnowledgeSpace.BackendServer.UnitTest.Controllers
{
    public class StatisticsControllerTest
    {
        private ApplicationDbContext _context;

        public StatisticsControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var controller = new StatisticsController(_context);
            Assert.NotNull(controller);
        }
    }
}