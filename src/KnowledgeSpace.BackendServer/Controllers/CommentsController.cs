using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Helpers;
using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public partial class KnowledgeBasesController
    {
        #region Comments

        [HttpGet("{knowledgeBaseId}/comments/filter")]
        [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
        public async Task<IActionResult> GetCommentsPaging(int knowledgeBaseId, string filter, int pageIndex, int pageSize)
        {
            var query = _context.Comments.Where(x => x.KnowledgeBaseId == knowledgeBaseId).AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Content.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(c => new CommentVm()
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreateDate = c.CreateDate,
                    KnowledgeBaseId = c.KnowledgeBaseId,
                    LastModifiedDate = c.LastModifiedDate,
                    OwnwerUserId = c.OwnwerUserId
                })
                .ToListAsync();

            var pagination = new Pagination<CommentVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{knowledgeBaseId}/comments/{commentId}")]
        [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
        public async Task<IActionResult> GetCommentDetail(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found comment with id: {commentId}"));

            var commentVm = new CommentVm()
            {
                Id = comment.Id,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                KnowledgeBaseId = comment.KnowledgeBaseId,
                LastModifiedDate = comment.LastModifiedDate,
                OwnwerUserId = comment.OwnwerUserId
            };

            return Ok(commentVm);
        }

        [HttpPost("{knowledgeBaseId}/comments")]
        [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostComment(int knowledgeBaseId, [FromBody]CommentCreateRequest request)
        {
            var comment = new Comment()
            {
                Content = request.Content,
                KnowledgeBaseId = request.KnowledgeBaseId,
                OwnwerUserId = string.Empty/*TODO: GET USER FROM CLAIM*/,
            };
            _context.Comments.Add(comment);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase != null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id: {knowledgeBaseId}"));

            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfVotes.GetValueOrDefault(0) + 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetCommentDetail), new { id = knowledgeBaseId, commentId = comment.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create comment failed"));
            }
        }

        [HttpPut("{knowledgeBaseId}/comments/{commentId}")]
        [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.UPDATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PutComment(int commentId, [FromBody]CommentCreateRequest request)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found comment with id: {commentId}"));
            if (comment.OwnwerUserId != User.Identity.Name)
                return Forbid();

            comment.Content = request.Content;
            _context.Comments.Update(comment);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse($"Update comment failed"));
        }

        [HttpDelete("{knowledgeBaseId}/comments/{commentId}")]
        [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteComment(int knowledgeBaseId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found the comment with id: {commentId}"));

            _context.Comments.Remove(comment);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase != null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id: {knowledgeBaseId}"));

            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfVotes.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var commentVm = new CommentVm()
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreateDate = comment.CreateDate,
                    KnowledgeBaseId = comment.KnowledgeBaseId,
                    LastModifiedDate = comment.LastModifiedDate,
                    OwnwerUserId = comment.OwnwerUserId
                };
                return Ok(commentVm);
            }
            return BadRequest(new ApiBadRequestResponse($"Delete comment failed"));
        }

        #endregion Comments
    }
}