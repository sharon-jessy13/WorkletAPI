using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using PrismWorkletApi.Models;

namespace PrismWorkletApi.Repositories
{
    public interface IMentorRepository
    {
        Task<IEnumerable<MentorSearchResult>> SearchAsync(string query);
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
            var parameters = new { SearchQuery = $"%{query}%" };
            return await conn.QueryAsync<MentorSearchResult>(
                "WFSRV_LiveEMPDetails",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}