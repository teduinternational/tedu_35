using KnowledgeSpace.WebPortal.Models;
using KnowledgeSpace.WebPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Controllers.Components
{
    public class SideBarViewComponent : ViewComponent
    {
        private ICategoryApiClient _categoryApiClient;
        private IKnowledgeBaseApiClient _knowledgeBaseApiClient;

        public SideBarViewComponent(ICategoryApiClient categoryApiClient,
            IKnowledgeBaseApiClient knowledgeBaseApiClient)
        {
            _categoryApiClient = categoryApiClient;
            _knowledgeBaseApiClient = knowledgeBaseApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryApiClient.GetCategories();
            var popularKnowledgeBases = await _knowledgeBaseApiClient.GetPopularKnowledgeBases(4);
            var recentComments = await _knowledgeBaseApiClient.GetRecentComments(4);
            var viewModel = new SideBarViewModel()
            {
                Categories = categories,
                PopularKnowledgeBases = popularKnowledgeBases,
                RecentComments = recentComments
            };
            return View("Default", viewModel);
        }
    }
}