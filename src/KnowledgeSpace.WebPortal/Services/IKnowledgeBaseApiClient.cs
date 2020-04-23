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

        Task<List<LabelVm>> GetPopularLabels(int take);
    }
}