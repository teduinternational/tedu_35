using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Services
{
    public interface IOneSignalService
    {
        Task SendAsync(string title, string message, string url);
    }
}