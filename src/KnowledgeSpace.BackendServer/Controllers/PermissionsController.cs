using Dapper;
using KnowledgeSpace.BackendServer.Authorization;
using KnowledgeSpace.BackendServer.Constants;
using KnowledgeSpace.ViewModels.Systems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public class PermissionsController : BaseController
    {
        private readonly IConfiguration _configuration;

        public PermissionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Show list function with corressponding action included in each functions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.VIEW)]
        public async Task<IActionResult> GetCommandViews()
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync();
                }

                var sql = @"SELECT f.Id,
	                       f.Name,
	                       f.ParentId,
	                       sum(case when sa.Id = 'CREATE' then 1 else 0 end) as HasCreate,
	                       sum(case when sa.Id = 'UPDATE' then 1 else 0 end) as HasUpdate,
	                       sum(case when sa.Id = 'DELETE' then 1 else 0 end) as HasDelete,
	                       sum(case when sa.Id = 'VIEW' then 1 else 0 end) as HasView,
	                       sum(case when sa.Id = 'APPROVE' then 1 else 0 end) as HasApprove
                        from Functions f join CommandInFunctions cif on f.Id = cif.FunctionId
		                    left join Commands sa on cif.CommandId = sa.Id
                        GROUP BY f.Id,f.Name, f.ParentId
                        order BY f.ParentId";

                var result = await conn.QueryAsync<PermissionScreenVm>(sql, null, null, 120, CommandType.Text);
                return Ok(result.ToList());
            }
        }
    }
}