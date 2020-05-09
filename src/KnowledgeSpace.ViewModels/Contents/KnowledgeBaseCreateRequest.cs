using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KnowledgeSpace.ViewModels.Contents
{
    public class KnowledgeBaseCreateRequest
    {
        public int? Id { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; }

        public string SeoAlias { get; set; }

        public string Description { get; set; }

        public string Environment { get; set; }

        public string Problem { get; set; }

        public string StepToReproduce { get; set; }

        public string ErrorMessage { get; set; }

        public string Workaround { get; set; }

        public string Note { get; set; }

        public string[] Labels { get; set; }

        public List<IFormFile> Attachments { get; set; }

        public string CaptchaCode { get; set; }
    }
}