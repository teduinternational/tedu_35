using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Extensions;
using KnowledgeSpace.BackendServer.Helpers;
using KnowledgeSpace.BackendServer.Services;
using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public partial class KnowledgeBasesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ISequenceService _sequenceService;
        private readonly IStorageService _storageService;
        private readonly ILogger<KnowledgeBasesController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICacheService _cacheService;
        private readonly IOneSignalService _oneSignalService;

        public KnowledgeBasesController(ApplicationDbContext context,
            ISequenceService sequenceService,
            IStorageService storageService,
            ILogger<KnowledgeBasesController> logger,
            IEmailSender emailSender,
            IViewRenderService viewRenderService,
            ICacheService cacheService,
            IOneSignalService oneSignalService)
        {
            _context = context;
            _sequenceService = sequenceService;
            _storageService = storageService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
            _cacheService = cacheService;
            _oneSignalService = oneSignalService;
        }

        [HttpPost]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.CREATE)]
        [ApiValidationFilter]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostKnowledgeBase([FromForm] KnowledgeBaseCreateRequest request)
        {
            _logger.LogInformation("Begin PostKnowledgeBase API");
            KnowledgeBase knowledgeBase = CreateKnowledgeBaseEntity(request);
            knowledgeBase.OwnerUserId = User.GetUserId();
            if (string.IsNullOrEmpty(knowledgeBase.SeoAlias))
            {
                knowledgeBase.SeoAlias = TextHelper.ToUnsignString(knowledgeBase.Title);
            }
            knowledgeBase.Id = await _sequenceService.GetKnowledgeBaseNewId();

            //Process attachment
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var attachment in request.Attachments)
                {
                    var attachmentEntity = await SaveFile(knowledgeBase.Id, attachment);
                    _context.Attachments.Add(attachmentEntity);
                }
            }
            _context.KnowledgeBases.Add(knowledgeBase);

            //Process label
            if (request.Labels?.Length > 0)
            {
                await ProcessLabel(request, knowledgeBase);
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync(CacheConstants.LatestKnowledgeBases);
                await _cacheService.RemoveAsync(CacheConstants.PopularKnowledgeBases);

                _logger.LogInformation("End PostKnowledgeBase API - Success");

                await _oneSignalService.SendAsync(request.Title, request.Description,
                     string.Format(CommonConstants.KnowledgeBaseUrl, knowledgeBase.SeoAlias, knowledgeBase.Id));

                return CreatedAtAction(nameof(GetById), new
                {
                    id = knowledgeBase.Id
                });
            }
            else
            {
                _logger.LogInformation("End PostKnowledgeBase API - Failed");

                return BadRequest(new ApiBadRequestResponse("Create knowledge failed"));
            }
        }

        [HttpGet]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.VIEW)]
        public async Task<IActionResult> GetKnowledgeBases()
        {
            var knowledgeBases = _context.KnowledgeBases;

            var knowledgeBasevms = await knowledgeBases.Select(u => new KnowledgeBaseQuickVm()
            {
                Id = u.Id,
                CategoryId = u.CategoryId,
                Description = u.Description,
                SeoAlias = u.SeoAlias,
                Title = u.Title
            }).ToListAsync();

            return Ok(knowledgeBasevms);
        }

        [HttpGet("latest/{take:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatestKnowledgeBases(int take)
        {
            var cachedData = await _cacheService.GetAsync<List<KnowledgeBaseQuickVm>>(CacheConstants.LatestKnowledgeBases);
            if (cachedData == null)
            {
                var knowledgeBases = from k in _context.KnowledgeBases
                                     join c in _context.Categories on k.CategoryId equals c.Id
                                     orderby k.CreateDate descending
                                     select new { k, c };

                var knowledgeBaseVms = await knowledgeBases.Take(take)
                    .Select(u => new KnowledgeBaseQuickVm()
                    {
                        Id = u.k.Id,
                        CategoryId = u.k.CategoryId,
                        Description = u.k.Description,
                        SeoAlias = u.k.SeoAlias,
                        Title = u.k.Title,
                        CategoryAlias = u.c.SeoAlias,
                        CategoryName = u.c.Name,
                        NumberOfVotes = u.k.NumberOfVotes,
                        CreateDate = u.k.CreateDate
                    }).ToListAsync();
                await _cacheService.SetAsync(CacheConstants.LatestKnowledgeBases, knowledgeBaseVms, 2);
                cachedData = knowledgeBaseVms;
            }

            return Ok(cachedData);
        }

        [HttpGet("popular/{take:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularKnowledgeBases(int take)
        {
            var cachedData = await _cacheService.GetAsync<List<KnowledgeBaseQuickVm>>(CacheConstants.PopularKnowledgeBases);
            if (cachedData == null)
            {
                var knowledgeBases = from k in _context.KnowledgeBases
                                     join c in _context.Categories on k.CategoryId equals c.Id
                                     orderby k.ViewCount descending
                                     select new { k, c };

                var knowledgeBaseVms = await knowledgeBases.Take(take)
                    .Select(u => new KnowledgeBaseQuickVm()
                    {
                        Id = u.k.Id,
                        CategoryId = u.k.CategoryId,
                        Description = u.k.Description,
                        SeoAlias = u.k.SeoAlias,
                        Title = u.k.Title,
                        CategoryAlias = u.c.SeoAlias,
                        CategoryName = u.c.Name,
                        NumberOfVotes = u.k.NumberOfVotes,
                        CreateDate = u.k.CreateDate
                    }).ToListAsync();
                await _cacheService.SetAsync(CacheConstants.PopularKnowledgeBases, knowledgeBaseVms, 24);
                cachedData = knowledgeBaseVms;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetKnowledgeBasesPaging(string filter, int? categoryId, int pageIndex, int pageSize)
        {
            var query = from k in _context.KnowledgeBases
                        join c in _context.Categories on k.CategoryId equals c.Id
                        select new { k, c };
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.k.Title.Contains(filter));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.k.CategoryId == categoryId.Value);
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new KnowledgeBaseQuickVm()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryAlias = u.c.SeoAlias,
                    CategoryName = u.c.Name,
                    NumberOfVotes = u.k.NumberOfVotes,
                    CreateDate = u.k.CreateDate,
                    NumberOfComments = u.k.NumberOfComments
                })
                .ToListAsync();

            var pagination = new Pagination<KnowledgeBaseQuickVm>
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("tags/{labelId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetKnowledgeBasesByTagId(string labelId, int pageIndex, int pageSize)
        {
            var query = from k in _context.KnowledgeBases
                        join lik in _context.LabelInKnowledgeBases on k.Id equals lik.KnowledgeBaseId
                        join l in _context.Labels on lik.LabelId equals l.Id
                        join c in _context.Categories on k.CategoryId equals c.Id
                        where lik.LabelId == labelId
                        select new { k, l, c };

            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new KnowledgeBaseQuickVm()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryAlias = u.c.SeoAlias,
                    CategoryName = u.c.Name,
                    NumberOfVotes = u.k.NumberOfVotes,
                    CreateDate = u.k.CreateDate,
                    NumberOfComments = u.k.NumberOfComments
                })
                .ToListAsync();

            var pagination = new Pagination<KnowledgeBaseQuickVm>
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id: {id}"));

            var attachments = await _context.Attachments
                .Where(x => x.KnowledgeBaseId == id)
                .Select(x => new AttachmentVm()
                {
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    FileSize = x.FileSize,
                    Id = x.Id,
                    FileType = x.FileType
                }).ToListAsync();
            var knowledgeBaseVm = CreateKnowledgeBaseVm(knowledgeBase);
            knowledgeBaseVm.Attachments = attachments;

            return Ok(knowledgeBaseVm);
        }

        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.UPDATE)]
        [ApiValidationFilter]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PutKnowledgeBase(int id, [FromForm]KnowledgeBaseCreateRequest request)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id {id}"));
            UpdateKnowledgeBase(request, knowledgeBase);

            //Process attachment
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var attachment in request.Attachments)
                {
                    var attachmentEntity = await SaveFile(knowledgeBase.Id, attachment);
                    _context.Attachments.Add(attachmentEntity);
                }
            }
            _context.KnowledgeBases.Update(knowledgeBase);

            if (request.Labels?.Length > 0)
            {
                await ProcessLabel(request, knowledgeBase);
            }
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("LatestKnowledgeBases");
                await _cacheService.RemoveAsync("PopularKnowledgeBases");
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse($"Update knowledge base failed"));
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteKnowledgeBase(int id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound();

            _context.KnowledgeBases.Remove(knowledgeBase);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync(CacheConstants.LatestKnowledgeBases);
                await _cacheService.RemoveAsync(CacheConstants.PopularKnowledgeBases);

                KnowledgeBaseVm knowledgeBasevm = CreateKnowledgeBaseVm(knowledgeBase);
                return Ok(knowledgeBasevm);
            }
            return BadRequest();
        }

        [HttpGet("{knowlegeBaseId}/labels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLabelsByKnowledgeBaseId(int knowlegeBaseId)
        {
            var query = from lik in _context.LabelInKnowledgeBases
                        join l in _context.Labels on lik.LabelId equals l.Id
                        orderby l.Name ascending
                        where lik.KnowledgeBaseId == knowlegeBaseId
                        select new { l.Id, l.Name };

            var labels = await query.Select(u => new LabelVm()
            {
                Id = u.Id,
                Name = u.Name
            }).ToListAsync();

            return Ok(labels);
        }

        [HttpPut("{id}/view-count")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateViewCount(int id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound();
            if (knowledgeBase.ViewCount == null)
                knowledgeBase.ViewCount = 0;

            knowledgeBase.ViewCount += 1;
            _context.KnowledgeBases.Update(knowledgeBase);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        #region Private methods

        private static KnowledgeBaseVm CreateKnowledgeBaseVm(KnowledgeBase knowledgeBase)
        {
            return new KnowledgeBaseVm()
            {
                Id = knowledgeBase.Id,

                CategoryId = knowledgeBase.CategoryId,

                Title = knowledgeBase.Title,

                SeoAlias = knowledgeBase.SeoAlias,

                Description = knowledgeBase.Description,

                Environment = knowledgeBase.Environment,

                Problem = knowledgeBase.Problem,

                StepToReproduce = knowledgeBase.StepToReproduce,

                ErrorMessage = knowledgeBase.ErrorMessage,

                Workaround = knowledgeBase.Workaround,

                Note = knowledgeBase.Note,

                OwnerUserId = knowledgeBase.OwnerUserId,

                Labels = !string.IsNullOrEmpty(knowledgeBase.Labels) ? knowledgeBase.Labels.Split(',') : null,

                CreateDate = knowledgeBase.CreateDate,

                LastModifiedDate = knowledgeBase.LastModifiedDate,

                NumberOfComments = knowledgeBase.NumberOfComments,

                NumberOfVotes = knowledgeBase.NumberOfVotes,

                NumberOfReports = knowledgeBase.NumberOfReports,
            };
        }

        private static KnowledgeBase CreateKnowledgeBaseEntity(KnowledgeBaseCreateRequest request)
        {
            var entity = new KnowledgeBase()
            {
                CategoryId = request.CategoryId,

                Title = request.Title,

                SeoAlias = request.SeoAlias,

                Description = request.Description,

                Environment = request.Environment,

                Problem = request.Problem,

                StepToReproduce = request.StepToReproduce,

                ErrorMessage = request.ErrorMessage,

                Workaround = request.Workaround,

                Note = request.Note
            };
            if (request.Labels?.Length > 0)
            {
                entity.Labels = string.Join(',', request.Labels);
            }
            return entity;
        }

        private async Task<Attachment> SaveFile(int knowledegeBaseId, IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{originalFileName.Substring(0, originalFileName.LastIndexOf('.'))}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            var attachmentEntity = new Attachment()
            {
                FileName = fileName,
                FilePath = _storageService.GetFileUrl(fileName),
                FileSize = file.Length,
                FileType = Path.GetExtension(fileName),
                KnowledgeBaseId = knowledegeBaseId,
            };
            return attachmentEntity;
        }

        private async Task ProcessLabel(KnowledgeBaseCreateRequest request, KnowledgeBase knowledgeBase)
        {
            foreach (var labelText in request.Labels)
            {
                if (labelText == null) continue;
                var labelId = TextHelper.ToUnsignString(labelText.ToString());
                var existingLabel = await _context.Labels.FindAsync(labelId);
                if (existingLabel == null)
                {
                    var labelEntity = new Label()
                    {
                        Id = labelId,
                        Name = labelText.ToString()
                    };
                    _context.Labels.Add(labelEntity);
                }
                if (await _context.LabelInKnowledgeBases.FindAsync(labelId, knowledgeBase.Id) == null)
                {
                    _context.LabelInKnowledgeBases.Add(new LabelInKnowledgeBase()
                    {
                        KnowledgeBaseId = knowledgeBase.Id,
                        LabelId = labelId
                    });
                }
            }
        }

        private static void UpdateKnowledgeBase(KnowledgeBaseCreateRequest request, KnowledgeBase knowledgeBase)
        {
            knowledgeBase.CategoryId = request.CategoryId;

            knowledgeBase.Title = request.Title;

            if (string.IsNullOrEmpty(request.SeoAlias))
                knowledgeBase.SeoAlias = TextHelper.ToUnsignString(request.Title);
            else
                knowledgeBase.SeoAlias = request.SeoAlias;

            knowledgeBase.Description = request.Description;

            knowledgeBase.Environment = request.Environment;

            knowledgeBase.Problem = request.Problem;

            knowledgeBase.StepToReproduce = request.StepToReproduce;

            knowledgeBase.ErrorMessage = request.ErrorMessage;

            knowledgeBase.Workaround = request.Workaround;

            knowledgeBase.Note = request.Note;

            if (request.Labels != null)
                knowledgeBase.Labels = string.Join(',', request.Labels);
        }

        #endregion Private methods
    }
}