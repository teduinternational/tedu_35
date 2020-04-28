using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeSpace.WebPortal.Extensions;
using KnowledgeSpace.WebPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeSpace.WebPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserApiClient _userApiClient;

        public AccountController(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
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
            return View(user);
        }
    }
}