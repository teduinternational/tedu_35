using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Contents
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }

        public string SeoAlias { get; set; }

        public string SeoDescription { get; set; }

        public int SortOrder { get; set; }

        public int? ParentId { get; set; }
    }
}