using KnowledgeSpace.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(_emailSettings.ApiBaseUri),
                Authenticator = new HttpBasicAuthenticator("api", _emailSettings.ApiKey)
            };
            RestRequest request = new RestRequest();
            request.AddParameter("domain", _emailSettings.Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", _emailSettings.From);
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("html", htmlMessage);
            request.Method = Method.POST;

            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();

            client.ExecuteAsync(
                request, r => taskCompletion.SetResult(r));

            RestResponse response = (RestResponse)(await taskCompletion.Task);
        }
    }
}