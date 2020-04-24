using KnowledgeSpace.ViewModels.Contents;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public class LabelApiClient : ILabelApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LabelApiClient(IHttpClientFactory httpClientFactory,
         IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<LabelVm> GetLabelById(string labelId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            var response = await client.GetAsync($"/api/labels/{labelId}");
            var label = JsonConvert.DeserializeObject<LabelVm>(await response.Content.ReadAsStringAsync());
            return label;
        }

        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            var response = await client.GetAsync($"/api/labels/popular/{take}");
            var labels = JsonConvert.DeserializeObject<List<LabelVm>>(await response.Content.ReadAsStringAsync());
            return labels;
        }
    }
}