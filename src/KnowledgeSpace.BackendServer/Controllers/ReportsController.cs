using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Extensions;
using KnowledgeSpace.BackendServer.Helpers;
using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public partial class KnowledgeBasesController
    {
        #region Reports

        [HttpGet("{knowledgeBaseId}/reports/filter")]
        public async Task<IActionResult> GetReportsPaging(int? knowledgeBaseId, string filter, int pageIndex, int pageSize)
        {
            var query = from r in _context.Reports
                        join u in _context.Users
                            on r.ReportUserId equals u.Id
                        select new { r, u };
            if (knowledgeBaseId.HasValue)
            {
                query = query.Where(x => x.r.KnowledgeBaseId == knowledgeBaseId.Value);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.r.Content.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ReportVm()
                {
                    Id = c.r.Id,
                    Content = c.r.Content,
                    CreateDate = c.r.CreateDate,
                    KnowledgeBaseId = c.r.KnowledgeBaseId,
                    LastModifiedDate = c.r.LastModifiedDate,
                    IsProcessed = false,
                    ReportUserId = c.r.ReportUserId,
                    ReportUserName = c.u.FirstName + " " + c.u.LastName
                })
                .ToListAsync();

            var pagination = new Pagination<ReportVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{knowledgeBaseId}/reports/{reportId}")]
        public async Task<IActionResult> GetReportDetail(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return NotFound();
            var user = await _context.Users.FindAsync(report.ReportUserId);

            var reportVm = new ReportVm()
            {
                Id = report.Id,
                Content = report.Content,
                CreateDate = report.CreateDate,
                KnowledgeBaseId = report.KnowledgeBaseId,
                LastModifiedDate = report.LastModifiedDate,
                IsProcessed = report.IsProcessed,
                ReportUserId = report.ReportUserId,
                ReportUserName = user.FirstName + " " + user.LastName
            };

            return Ok(reportVm);
        }

        [HttpPost("{knowledgeBaseId}/reports")]
        [ApiValidationFilter]
        public async Task<IActionResult> PostReport(int knowledgeBaseId, [FromBody]ReportCreateRequest request)
        {
            var report = new Report()
            {
                Content = request.Content,
                KnowledgeBaseId = knowledgeBaseId,
                ReportUserId = User.GetUserId(),
                IsProcessed = false
            };
            _context.Reports.Add(report);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id {knowledgeBaseId}"));

            knowledgeBase.NumberOfReports = knowledgeBase.NumberOfReports.GetValueOrDefault(0) + 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse($"Create report failed"));
            }
        }

        [HttpDelete("{knowledgeBaseId}/reports/{reportId}")]
        public async Task<IActionResult> DeleteReport(int knowledgeBaseId, int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found report with id {reportId}"));

            _context.Reports.Remove(report);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id {knowledgeBaseId}"));

            knowledgeBase.NumberOfReports = knowledgeBase.NumberOfReports.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest(new ApiBadRequestResponse($"Delete report failed"));
        }

        #endregion Reports
    }
}