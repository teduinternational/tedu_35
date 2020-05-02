using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using KnowledgeSpace.ViewModels.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public interface IUserApiClient
    {
        Task<UserVm> GetById(string id);

        Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByUserId(string userId, int pageIndex, int pageSize);
    }
}