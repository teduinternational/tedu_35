using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.ViewModels.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public class StatisticsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("monthly-comments")]
        [ClaimRequirement(FunctionCode.STATISTIC, CommandCode.VIEW)]
        public async Task<IActionResult> GetMonthlyNewComments(int year)
        {
            var data = await _context.Comments.Where(x => x.CreateDate.Date.Year == year)
                .GroupBy(x => x.CreateDate.Date.Month)
                .OrderBy(x => x.Key)
                .Select(g => new MonthlyCommentsVm()
                {
                    Month = g.Key,
                    NumberOfComments = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("monthly-newkbs")]
        [ClaimRequirement(FunctionCode.STATISTIC, CommandCode.VIEW)]
        public async Task<IActionResult> GetMonthlyNewKbs(int year)
        {
            var data = await _context.KnowledgeBases.Where(x => x.CreateDate.Date.Year == year)
                .GroupBy(x => x.CreateDate.Date.Month)
                .Select(g => new MonthlyNewKbsVm()
                {
                    Month = g.Key,
                    NumberOfNewKbs = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("monthly-registers")]
        [ClaimRequirement(FunctionCode.STATISTIC, CommandCode.VIEW)]
        public async Task<IActionResult> GetMonthlyNewRegisters(int year)
        {
            var data = await _context.Users.Where(x => x.CreateDate.Date.Year == year)
               .GroupBy(x => x.CreateDate.Date.Month)
               .Select(g => new MonthlyNewKbsVm()
               {
                   Month = g.Key,
                   NumberOfNewKbs = g.Count()
               })
               .ToListAsync();

            return Ok(data);
        }
    }
}