using KnowledgeSpace.ViewModels.Contents;
using KnowledgeSpace.ViewModels.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Models
{
    public class KnowledgeBaseDetailViewModel
    {
        public CategoryVm Category { set; get; }
        public KnowledgeBaseVm Detail { get; set; }

        public List<LabelVm> Labels { get; set; }

        public UserVm CurrentUser { get; set; }
    }
}