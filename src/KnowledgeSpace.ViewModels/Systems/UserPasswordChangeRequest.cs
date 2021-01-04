using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Systems
{
    public class UserPasswordChangeRequest
    {
        public string UserId { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}