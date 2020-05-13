using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public interface IKnowledgeBaseApiClient
    {
        Task<List<KnowledgeBaseQuickVm>> GetPopularKnowledgeBases(int take);

        Task<List<KnowledgeBaseQuickVm>> GetLatestKnowledgeBases(int take);

        Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByCategoryId(int categoryId, int pageIndex, int pageSize);

        Task<Pagination<KnowledgeBaseQuickVm>> SearchKnowledgeBase(string keyword, int pageIndex, int pageSize);

        Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByTagId(string tagId, int pageIndex, int pageSize);

        Task<KnowledgeBaseVm> GetKnowledgeBaseDetail(int id);

        Task<List<LabelVm>> GetLabelsByKnowledgeBaseId(int id);

        Task<List<CommentVm>> GetRecentComments(int take);

        Task<Pagination<CommentVm>> GetCommentsTree(int knowledgeBaseId, int pageIndex, int pageSize);

        Task<Pagination<CommentVm>> GetRepliedComments(int knowledgeBaseId, int rootCommentId, int pageIndex, int pageSize);

        Task<CommentVm> PostComment(CommentCreateRequest request);

        Task<bool> PostKnowlegdeBase(KnowledgeBaseCreateRequest request);

        Task<bool> PutKnowlegdeBase(int id, KnowledgeBaseCreateRequest request);

        Task<bool> UpdateViewCount(int id);

        Task<int> PostVote(VoteCreateRequest request);

        Task<ReportVm> PostReport(ReportCreateRequest request);
    }
}