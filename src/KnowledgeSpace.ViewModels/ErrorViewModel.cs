using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}