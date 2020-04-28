using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Extensions;
using KnowledgeSpace.BackendServer.Helpers;
using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public partial class KnowledgeBasesController
    {
        #region Comments

        [HttpGet("{knowledgeBaseId}/comments/filter")]
        [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
        public async Task<IActionResult> GetCommentsPaging(int? knowledgeBaseId, string filter, int pageIndex, int pageSize)
        {
            var query = from c in _context.Comments
                        join u in _context.Users
                            on c.OwnerUserId equals u.Id
                        select new { c, u };
            if (knowledgeBaseId.HasValue)
            {
                query = query.Where(x => x.c.KnowledgeBaseId == knowledgeBaseId.Value);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.c.Content.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentVm()
                {
                    Id = c.c.Id,
                    Content = c.c.Content,
                    CreateDate = c.c.CreateDate,
                    KnowledgeBaseId = c.c.KnowledgeBaseId,
                    LastModifiedDate = c.c.LastModifiedDate,
                    OwnerUserId = c.c.OwnerUserId,
                    OwnerName = c.u.FirstName + " " + c.u.LastName
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
            var user = await _context.Users.FindAsync(comment.OwnerUserId);
            var commentVm = new CommentVm()
            {
                Id = comment.Id,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                KnowledgeBaseId = comment.KnowledgeBaseId,
                LastModifiedDate = comment.LastModifiedDate,
                OwnerUserId = comment.OwnerUserId,
                OwnerName = user.FirstName + " " + user.LastName
            };

            return Ok(commentVm);
        }

        [HttpPost("{knowledgeBaseId}/comments")]
        [ApiValidationFilter]
        public async Task<IActionResult> PostComment(int knowledgeBaseId, [FromBody]CommentCreateRequest request)
        {
            var comment = new Comment()
            {
                Content = request.Content,
                KnowledgeBaseId = knowledgeBaseId,
                OwnerUserId = User.GetUserId(),
                ReplyId = request.ReplyId
            };
            _context.Comments.Add(comment);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id: {knowledgeBaseId}"));

            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfComments.GetValueOrDefault(0) + 1;
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
            if (comment.OwnerUserId != User.GetUserId())
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
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id: {knowledgeBaseId}"));

            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfComments.GetValueOrDefault(0) - 1;
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
                    OwnerUserId = comment.OwnerUserId
                };
                return Ok(commentVm);
            }
            return BadRequest(new ApiBadRequestResponse($"Delete comment failed"));
        }

        [HttpGet("comments/recent/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentComments(int take)
        {
            var query = from c in _context.Comments
                        join u in _context.Users
                            on c.OwnerUserId equals u.Id
                        join k in _context.KnowledgeBases
                        on c.KnowledgeBaseId equals k.Id
                        orderby c.CreateDate descending
                        select new { c, u, k };

            var comments = await query.Take(take).Select(x => new CommentVm()
            {
                Id = x.c.Id,
                CreateDate = x.c.CreateDate,
                KnowledgeBaseId = x.c.KnowledgeBaseId,
                OwnerUserId = x.c.OwnerUserId,
                KnowledgeBaseTitle = x.k.Title,
                OwnerName = x.u.FirstName + " " + x.u.LastName,
                KnowledgeBaseSeoAlias = x.k.SeoAlias
            }).ToListAsync();

            return Ok(comments);
        }

        [HttpGet("{knowledgeBaseId}/comments/tree")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentTreeByKnowledgeBaseId(int knowledgeBaseId)
        {
            var query = from c in _context.Comments
                        join u in _context.Users
                            on c.OwnerUserId equals u.Id
                        where c.KnowledgeBaseId == knowledgeBaseId
                        select new { c, u };

            var flatComments = await query.Select(x => new CommentVm()
            {
                Id = x.c.Id,
                Content = x.c.Content,
                CreateDate = x.c.CreateDate,
                KnowledgeBaseId = x.c.KnowledgeBaseId,
                OwnerUserId = x.c.OwnerUserId,
                OwnerName = x.u.FirstName + " " + x.u.LastName,
                ReplyId = x.c.ReplyId
            }).ToListAsync();

            var lookup = flatComments.ToLookup(c => c.ReplyId);
            var rootCategories = flatComments.Where(x => x.ReplyId == null);

            foreach (var c in rootCategories)//only loop through root categories
            {
                // you can skip the check if you want an empty list instead of null
                // when there is no children
                if (lookup.Contains(c.Id))
                    c.Children = lookup[c.Id].ToList();
            }

            return Ok(rootCategories);
        }

        #endregion Comments
    }
}