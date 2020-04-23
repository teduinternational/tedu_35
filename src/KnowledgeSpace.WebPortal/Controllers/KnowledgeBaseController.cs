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
        private readonly ILabelApiClient _labelApiClient;

        public KnowledgeBaseController(IKnowledgeBaseApiClient knowledgeBaseApiClient,
            ICategoryApiClient categoryApiClient,
            ILabelApiClient labelApiClient,
            IConfiguration configuration)
        {
            _knowledgeBaseApiClient = knowledgeBaseApiClient;
            _categoryApiClient = categoryApiClient;
            _configuration = configuration;
            _labelApiClient = labelApiClient;
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

        public async Task<IActionResult> Search(string keyword, int page = 1)
        {
            var pageSize = int.Parse(_configuration["PageSize"]);
            var data = await _knowledgeBaseApiClient.SearchKnowledgeBase(keyword, page, pageSize);
            var viewModel = new SearchKnowledgeBaseViewModel()
            {
                Data = data,
                Keyword = keyword
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ListByTag(string tagId, int page = 1)
        {
            var pageSize = int.Parse(_configuration["PageSize"]);
            var data = await _knowledgeBaseApiClient.GetKnowledgeBasesByTagId(tagId, page, pageSize);
            var label = await _labelApiClient.GetLabelById(tagId);
            var viewModel = new ListByTagIdViewModel()
            {
                Data = data,
                LabelVm = label
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var knowledgeBase = await _knowledgeBaseApiClient.GetKnowledgeBaseDetail(id);
            var category = await _categoryApiClient.GetCategoryById(knowledgeBase.CategoryId);
            var labels = await _knowledgeBaseApiClient.GetLabelsByKnowledgeBaseId(id);
            var viewModel = new KnowledgeBaseDetailViewModel()
            {
                Detail = knowledgeBase,
                Category = category,
                Labels = labels
            };
            return View(viewModel);
        }
    }
}