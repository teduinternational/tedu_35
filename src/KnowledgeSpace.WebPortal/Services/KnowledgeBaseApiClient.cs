using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public class KnowledgeBaseApiClient : IKnowledgeBaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KnowledgeBaseApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<KnowledgeBaseQuickVm>> GetLatestKnowledgeBases(int take)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"/api/knowledgeBases/latest/{take}");
            var latestKnowledgeBases = JsonConvert.DeserializeObject<List<KnowledgeBaseQuickVm>>(await response.Content.ReadAsStringAsync());
            return latestKnowledgeBases;
        }

        public async Task<List<KnowledgeBaseQuickVm>> GetPopularKnowledgeBases(int take)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"/api/knowledgeBases/popular/{take}");
            var latestKnowledgeBases = JsonConvert.DeserializeObject<List<KnowledgeBaseQuickVm>>(await response.Content.ReadAsStringAsync());
            return latestKnowledgeBases;
        }

        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"/api/labels/popular/{take}");
            var labels = JsonConvert.DeserializeObject<List<LabelVm>>(await response.Content.ReadAsStringAsync());
            return labels;
        }
    }
}