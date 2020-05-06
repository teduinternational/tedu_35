using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.BackendServer.Helpers;
using KnowledgeSpace.BackendServer.Services;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public class LabelsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public LabelsController(ApplicationDbContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var label = await _context.Labels.FindAsync(id);
            if (label == null)
                return NotFound(new ApiNotFoundResponse($"Label with id: {id} is not found"));

            var labelVm = new LabelVm()
            {
                Id = label.Id,
                Name = label.Name
            };

            return Ok(labelVm);
        }

        [HttpGet("popular/{take:int}")]
        [AllowAnonymous]
        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            var cachedData = await _cacheService.GetAsync<List<LabelVm>>(CacheConstants.PopularLabels);
            if (cachedData == null)
            {
                var query = from l in _context.Labels
                            join lik in _context.LabelInKnowledgeBases on l.Id equals lik.LabelId
                            group new { l.Id, l.Name } by new { l.Id, l.Name } into g
                            select new
                            {
                                g.Key.Id,
                                g.Key.Name,
                                Count = g.Count()
                            };
                var labels = await query.OrderByDescending(x => x.Count).Take(take)
                    .Select(l => new LabelVm()
                    {
                        Id = l.Id,
                        Name = l.Name
                    }).ToListAsync();
                await _cacheService.SetAsync(CacheConstants.PopularLabels, labels);
                cachedData = labels;
            }

            return cachedData;
        }
    }
}