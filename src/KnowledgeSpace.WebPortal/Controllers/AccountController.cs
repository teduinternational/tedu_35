using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.ViewModels.Contents;
using KnowledgeSpace.WebPortal.Extensions;
using KnowledgeSpace.WebPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KnowledgeSpace.WebPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IKnowledgeBaseApiClient _knowledgeBaseApiClient;
        private readonly ICategoryApiClient _categoryApiClient;

        public AccountController(IUserApiClient userApiClient,
            IKnowledgeBaseApiClient knowledgeBaseApiClient,
            ICategoryApiClient categoryApiClient)
        {
            _userApiClient = userApiClient;
            _categoryApiClient = categoryApiClient;
            _knowledgeBaseApiClient = knowledgeBaseApiClient;
        }

        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "oidc");
        }

        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Cookies", "oidc");
        }

        [Authorize]
        public async Task<ActionResult> MyProfile()
        {
            var user = await _userApiClient.GetById(User.GetUserId());
            return View(user); ;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyKnowledgeBases(int page = 1, int pageSize = 10)
        {
            var kbs = await _userApiClient.GetKnowledgeBasesByUserId(User.GetUserId(), page, pageSize);
            return View(kbs);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewKnowledgeBase()
        {
            await SetCategoriesViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewKnowledgeBase([FromForm]KnowledgeBaseCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _knowledgeBaseApiClient.PostKnowlegdeBase(request);
            if (result)
            {
                return Ok();
            }
            return BadRequest(request);
        }

        [HttpGet]
        public async Task<IActionResult> EditKnowledgeBase(int id)
        {
            var knowledgeBase = await _knowledgeBaseApiClient.GetKnowledgeBaseDetail(id);
            await SetCategoriesViewBag();
            return View(knowledgeBase);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> EditKnowledgeBase(int id, [FromForm]KnowledgeBaseCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                await SetCategoriesViewBag();
                return View();
            }

            var result = await _knowledgeBaseApiClient.PostKnowlegdeBase(request);
            if (result)
            {
                TempData["message"] = "Thêm bài viết thành công";
                return Redirect("/my-kbs");
            }

            await SetCategoriesViewBag();
            return View(request);
        }

        private async Task SetCategoriesViewBag()
        {
            var categories = await _categoryApiClient.GetCategories();
            categories.Insert(0, new CategoryVm()
            {
                Id = 0,
                Name = "--Hãy chọn danh mục--"
            });
            ViewBag.Categories = categories;
        }
    }
}