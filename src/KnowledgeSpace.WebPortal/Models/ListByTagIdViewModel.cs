using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Models
{
    public class ListByTagIdViewModel
    {
        public Pagination<KnowledgeBaseQuickVm> Data { set; get; }

        public LabelVm LabelVm { set; get; }
    }
}