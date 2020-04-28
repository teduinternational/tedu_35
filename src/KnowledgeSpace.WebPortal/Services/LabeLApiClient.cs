using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public class LabelApiClient : BaseApiClient, ILabelApiClient
    {
        public LabelApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<LabelVm> GetLabelById(string labelId)
        {
            return await GetAsync<LabelVm>($"/api/labels/{labelId}");
        }

        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            return await GetListAsync<LabelVm>($"/api/labels/popular/{take}");
        }
    }
}