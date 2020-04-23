using KnowledgeSpace.ViewModels.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Models
{
    public class HomeViewModel
    {
        public List<KnowledgeBaseQuickVm> LatestKnowledgeBases { get; set; }
        public List<KnowledgeBaseQuickVm> PopularKnowledgeBases { get; set; }

        public List<LabelVm> PopularLabels { get; set; }
    }
}