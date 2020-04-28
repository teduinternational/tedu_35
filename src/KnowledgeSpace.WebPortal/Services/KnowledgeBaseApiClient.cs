using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public class KnowledgeBaseApiClient : BaseApiClient, IKnowledgeBaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KnowledgeBaseApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CommentVm>> GetCommentsTree(int knowledgeBaseId)
        {
            return await GetListAsync<CommentVm>($"/api/knowledgeBases/{knowledgeBaseId}/comments/tree");
        }

        public async Task<KnowledgeBaseVm> GetKnowledgeBaseDetail(int id)
        {
            return await GetAsync<KnowledgeBaseVm>($"/api/knowledgeBases/{id}");
        }

        public async Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            var apiUrl = $"/api/knowledgeBases/filter?categoryId={categoryId}&pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickVm>>(apiUrl);
        }

        public async Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByTagId(string tagId, int pageIndex, int pageSize)
        {
            var apiUrl = $"/api/knowledgeBases/tags/{tagId}?pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickVm>>(apiUrl);
        }

        public async Task<List<LabelVm>> GetLabelsByKnowledgeBaseId(int id)
        {
            return await GetListAsync<LabelVm>($"/api/knowledgeBases/{id}/labels");
        }

        public async Task<List<KnowledgeBaseQuickVm>> GetLatestKnowledgeBases(int take)
        {
            return await GetListAsync<KnowledgeBaseQuickVm>($"/api/knowledgeBases/latest/{take}");
        }

        public async Task<List<KnowledgeBaseQuickVm>> GetPopularKnowledgeBases(int take)
        {
            return await GetListAsync<KnowledgeBaseQuickVm>($"/api/knowledgeBases/popular/{take}");
        }

        public async Task<List<CommentVm>> GetRecentComments(int take)
        {
            return await GetListAsync<CommentVm>($"/api/knowledgeBases/comments/recent/{take}");
        }

        public async Task<bool> PostComment(CommentCreateRequest request)
        {
            return await PostAsync($"/api/knowledgeBases/{request.KnowledgeBaseId}/comments", request);
        }

        public async Task<bool> PostKnowlegdeBase(KnowledgeBaseCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            using var form = new MultipartFormDataContent();
            if (request.Attachments?.Count > 0)
            {
                foreach (var file in request.Attachments)
                {
                    form.Add(new StreamContent(file.OpenReadStream()));
                }
            }
            form.Add(new StringContent("CategoryId"), request.CategoryId.ToString());
            form.Add(new StringContent("Title"), request.Title.ToString());

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/knowledgeBases/", form);
            return response.IsSuccessStatusCode;
        }

        public async Task<Pagination<KnowledgeBaseQuickVm>> SearchKnowledgeBase(string keyword, int pageIndex, int pageSize)
        {
            var apiUrl = $"/api/knowledgeBases/filter?filter={keyword}&pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickVm>>(apiUrl);
        }
    }
}