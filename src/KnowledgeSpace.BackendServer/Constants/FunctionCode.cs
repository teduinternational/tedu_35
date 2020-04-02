using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Constants
{
    public enum FunctionCode
    {
        DASHBOARD,

        CONTENT,
        CONTENT_CATEGORY,
        CONTENT_KNOWLEDGEBASE,
        CONTENT_COMMENT,
        CONTENT_REPORT,

        STATISTIC,
        STATISTIC_MONTHLY_NEWMEMBER,
        STATISTIC_MONTHLY_NEWKB,
        STATISTIC_MONTHLY_COMMENT,

        SYSTEM,
        SYSTEM_USER,
        SYSTEM_ROLE,
        SYSTEM_FUNCTION,
        SYSTEM_PERMISSION,
    }
}