using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Contents
{
    public class ReportVm
    {
        public int Id { get; set; }

        public int? KnowledgeBaseId { get; set; }

        public string Content { get; set; }

        public string ReportUserId { get; set; }

        public string ReportUserName { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public bool IsProcessed { get; set; }

        public string Type { get; set; }
    }
}