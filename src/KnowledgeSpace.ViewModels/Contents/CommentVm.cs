using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Contents
{
    public class CommentVm
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int KnowledgeBaseId { get; set; }

        public string KnowledgeBaseTitle { get; set; }

        public string KnowledgeBaseSeoAlias { get; set; }

        public string OwnerUserId { get; set; }

        public string OwnerName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? ReplyId { get; set; }

        public Pagination<CommentVm> Children { get; set; } = new Pagination<CommentVm>();
    }
}