using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Helpers;
using KnowledgeSpace.BackendServer.Services;
using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public KnowledgeBasesController(ApplicationDbContext context,
            ISequenceService sequenceService,
            IStorageService storageService)
        {
            _context = context;
            _sequenceService = sequenceService;
            _storageService = storageService;
        }

        [HttpPost]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostKnowledgeBase([FromForm] KnowledgeBaseCreateRequest request)
        {
            KnowledgeBase knowledgeBase = CreateKnowledgeBaseEntity(request);
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
            if (!string.IsNullOrEmpty(request.Labels))
            {
                await ProcessLabel(request, knowledgeBase);
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return CreatedAtAction(nameof(GetById), new
                {
                    id = knowledgeBase.Id
                }, request);
            }
            else
            {
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

        [HttpGet("filter")]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.VIEW)]
        public async Task<IActionResult> GetKnowledgeBasesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.KnowledgeBases.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Title.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(u => new KnowledgeBaseQuickVm()
                {
                    Id = u.Id,
                    CategoryId = u.CategoryId,
                    Description = u.Description,
                    SeoAlias = u.SeoAlias,
                    Title = u.Title
                })
                .ToListAsync();

            var pagination = new Pagination<KnowledgeBaseQuickVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.VIEW)]
        public async Task<IActionResult> GetById(int id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id: {id}"));

            var knowledgeBasevm = CreateKnowledgeBaseVm(knowledgeBase);

            return Ok(knowledgeBasevm);
        }

        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.UPDATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PutKnowledgeBase(int id, [FromBody]KnowledgeBaseCreateRequest request)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id {id}"));
            UpdateKnowledgeBase(request, knowledgeBase);

            _context.KnowledgeBases.Update(knowledgeBase);

            if (!string.IsNullOrEmpty(request.Labels))
            {
                await ProcessLabel(request, knowledgeBase);
            }
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse($"Update knowledge base failed"));
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGEBASE, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteKnowledgeBase(string id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound();

            _context.KnowledgeBases.Remove(knowledgeBase);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                KnowledgeBaseVm knowledgeBasevm = CreateKnowledgeBaseVm(knowledgeBase);
                return Ok(knowledgeBasevm);
            }
            return BadRequest();
        }

        #region Private methods

        private static KnowledgeBaseVm CreateKnowledgeBaseVm(KnowledgeBase knowledgeBase)
        {
            return new KnowledgeBaseVm()
            {
                Id = knowledgeBase.CategoryId,

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

                Labels = knowledgeBase.Labels,

                CreateDate = knowledgeBase.CreateDate,

                LastModifiedDate = knowledgeBase.LastModifiedDate,

                NumberOfComments = knowledgeBase.CategoryId,

                NumberOfVotes = knowledgeBase.CategoryId,

                NumberOfReports = knowledgeBase.CategoryId,
            };
        }

        private static KnowledgeBase CreateKnowledgeBaseEntity(KnowledgeBaseCreateRequest request)
        {
            return new KnowledgeBase()
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

                Note = request.Note,

                Labels = request.Labels,
            };
        }

        private async Task<Attachment> SaveFile(int knowledegeBaseId, IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
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
            string[] labels = request.Labels.Split(',');
            foreach (var labelText in labels)
            {
                var labelId = TextHelper.ToUnsignString(labelText);
                var existingLabel = await _context.Labels.FindAsync(labelId);
                if (existingLabel == null)
                {
                    var labelEntity = new Label()
                    {
                        Id = labelId,
                        Name = labelText
                    };
                    _context.Labels.Add(labelEntity);
                }
                var labelInKnowledgeBase = new LabelInKnowledgeBase()
                {
                    KnowledgeBaseId = knowledgeBase.Id,
                    LabelId = labelId
                };
                _context.LabelInKnowledgeBases.Add(labelInKnowledgeBase);
            }
        }

        private static void UpdateKnowledgeBase(KnowledgeBaseCreateRequest request, KnowledgeBase knowledgeBase)
        {
            knowledgeBase.CategoryId = request.CategoryId;

            knowledgeBase.Title = request.Title;

            knowledgeBase.SeoAlias = request.SeoAlias;

            knowledgeBase.Description = request.Description;

            knowledgeBase.Environment = request.Environment;

            knowledgeBase.Problem = request.Problem;

            knowledgeBase.StepToReproduce = request.StepToReproduce;

            knowledgeBase.ErrorMessage = request.ErrorMessage;

            knowledgeBase.Workaround = request.Workaround;

            knowledgeBase.Note = request.Note;

            knowledgeBase.Labels = request.Labels;
        }

        #endregion Private methods
    }
}