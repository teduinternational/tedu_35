using KnowledgeSpace.ViewModels.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public interface ILabelApiClient
    {
        Task<List<LabelVm>> GetPopularLabels(int take);

        Task<LabelVm> GetLabelById(string labelId);
    }
}