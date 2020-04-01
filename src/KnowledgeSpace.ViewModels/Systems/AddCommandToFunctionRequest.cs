using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Systems
{
    public class AddCommandToFunctionRequest
    {
        public string CommandId { get; set; }

        public string FunctionId { get; set; }
    }
}