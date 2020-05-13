using KnowledgeSpace.ViewModels.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Models
{
    public class SideBarViewModel
    {
        public List<KnowledgeBaseQuickVm> PopularKnowledgeBases { get; set; }

        public List<CategoryVm> Categories { get; set; }

        public List<CommentVm> RecentComments { get; set; }
    }
}