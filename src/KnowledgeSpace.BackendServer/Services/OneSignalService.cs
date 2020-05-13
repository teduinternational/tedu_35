using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Services
{
    public class OneSignalService : IOneSignalService
    {
        private readonly IConfiguration _configuration;

        public OneSignalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string title, string message, string url)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + _configuration["OneSignal:AppSecret"]);

            var obj = new
            {
                app_id = _configuration["OneSignal:AppId"],
                headings = new { en = title },
                contents = new { en = message },
                included_segments = new[] { "All" },
                url
            };
            var param = JsonConvert.SerializeObject(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = await request.GetRequestStreamAsync())
                {
                    await writer.WriteAsync(byteArray, 0, byteArray.Length);
                }

                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }
        }
    }
}