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
    public class CategoryApiClient : ICategoryApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CategoryVm>> GetCategories()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/categories");
            var categories = JsonConvert.DeserializeObject<List<CategoryVm>>(await response.Content.ReadAsStringAsync());
            return categories;
        }

        public async Task<CategoryVm> GetCategoryById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            var response = await client.GetAsync($"/api/categories/{id}");
            var category = JsonConvert.DeserializeObject<CategoryVm>(await response.Content.ReadAsStringAsync());
            return category;
        }
    }
}