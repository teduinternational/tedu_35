using KnowledgeSpace.BackendServer.Data;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeSpace.BackendServer.UnitTest
{
    public class InMemoryDbContextFactory
    {
        public ApplicationDbContext GetApplicationDbContext(string databaseName = "InMemoryApplicationDatabase")
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                       .UseInMemoryDatabase(databaseName: databaseName)
                       .Options;
            var dbContext = new ApplicationDbContext(options);
            if (dbContext != null)
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }
            return dbContext;
        }
    }
}