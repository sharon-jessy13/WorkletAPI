using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using PrismWorkletApi.Models;

namespace PrismWorkletApi.Repositories
{
    public interface IMentorRepository
    {
        Task<IEnumerable<MentorSearchResult>> SearchAsync(string query);

        //Task<IEnumerable<CollegeModel>> GetCollegesAsync(int initiatorMEmpId, int instanceId);
    }

    public sealed class MentorRepository : IMentorRepository
    {
        private readonly string _connectionString;

        public MentorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<MentorSearchResult>> SearchAsync(string query)
        {
            using var conn = new SqlConnection(_connectionString);


            var allMentors = await conn.QueryAsync<MentorSearchResult>(
                "WFSRV_liveEMPDetails",
                null,
                commandType: CommandType.StoredProcedure);


            if (!string.IsNullOrEmpty(query))
            {

                return allMentors.Where(m =>
                    m.FullName.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return allMentors;
        }

        // public async Task<IEnumerable<CollegeModel>> GetCollegesAsync(int initiatorMEmpId, int instanceId)
        // {
        //     using var conn = new SqlConnection(_connectionString);
        
        //     // Pass the parameters to the stored procedure
        //     return await conn.QueryAsync<CollegeModel>(
        //         "PRISMWorklet_GetCollegeDetails",
        //         new { InitiatorMEmpID = initiatorMEmpId, InstanceID = instanceId },
        //         commandType: CommandType.StoredProcedure);
        // }

    }
}