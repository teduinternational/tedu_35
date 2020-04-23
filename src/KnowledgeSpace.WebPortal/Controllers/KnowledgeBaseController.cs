using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.WebPortal.Models;
using KnowledgeSpace.WebPortal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KnowledgeSpace.WebPortal.Controllers
{
    public class KnowledgeBaseController : Controller
    {
        private readonly IKnowledgeBaseApiClient _knowledgeBaseApiClient;
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly IConfiguration _configuration;

        public KnowledgeBaseController(IKnowledgeBaseApiClient knowledgeBaseApiClient,
            ICategoryApiClient categoryApiClient,
            IConfiguration configuration)
        {
            _knowledgeBaseApiClient = knowledgeBaseApiClient;
            _categoryApiClient = categoryApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> ListByCategoryId(int id, int page = 1)
        {
            var pageSize = int.Parse(_configuration["PageSize"]);
            var category = await _categoryApiClient.GetCategoryById(id);
            var data = await _knowledgeBaseApiClient.GetKnowledgeBasesByCategoryId(id, page, pageSize);
            var viewModel = new ListByCategoryIdViewModel()
            {
                Data = data,
                Category = category
            };
            return View(viewModel);
        }
    }
}