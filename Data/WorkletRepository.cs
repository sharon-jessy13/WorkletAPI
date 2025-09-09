using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using PrismWorkletApi.Models;

namespace PrismWorkletApi.Repositories
{
    public interface IWorkletRepository
    {
        Task<int> CreateWorkletAsync(WorkletCreateModel model);
        // Task<WorkletDetailsModel?> GetWorkletDetailsAsync(int initiatorMEmpID, int instanceID);
        // Task<IEnumerable<AttachmentModel>> GetAttachmentDetailsAsync(int initiatorMEmpID, int instanceID);
        // Task<IEnumerable<WorkletMentorDetailsModel>> GetMentorDetailsAsync(int initiatorMEmpID, int instanceID);

        Task<WorkletFullDetailsModel?> GetFullWorkletDetailsAsync(int initiatorMEmpId, int instanceId);
    }



    public sealed class WorkletRepository : IWorkletRepository
    {
        private readonly string _connectionString;

        public WorkletRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<int> CreateWorkletAsync(WorkletCreateModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var tran = conn.BeginTransaction();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@InitiatorMEmpID", model.InitiatorMEmpId);
                parameters.Add("@GroupMGID", 0);
                parameters.Add("@TeamMGID", 0);
                parameters.Add("@WCID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await conn.ExecuteAsync("PRISMWorkletCreation_Master_Insert", parameters, transaction: tran, commandType: CommandType.StoredProcedure);
                int workletId = parameters.Get<int>("@WCID");

                var primaryMentor = model.Mentors.FirstOrDefault(m => m.IsPrimary);
                if (primaryMentor == null) throw new InvalidOperationException("Primary mentor not found.");

                await conn.ExecuteAsync("PRISMWorklet_InsertMaster", new
                {
                    WorkletID = workletId,
                    model.Title,
                    ProblemStmt = model.ProblemStatement,
                    Prerequest = model.Prerequisites,
                    CreatedMentorID = primaryMentor.MentorId,
                    CreatedMentor = primaryMentor.MentorName,
                    model.StartDate,
                    model.EndDate,
                    model.StudentCount,
                    GitHubUrl = model.GitHubUrl ?? "",
                    model.Research,
                    model.POC,
                    LinkedProject = model.IsLinkedProject,

                    ProjectID = model.IsLinkedProject ? model.ProjectID : 0,

                    model.DomainID,
                    model.OtherDomain,
                    IsActive = true,
                    IsSync = false,
                    CreatedOn = DateTime.UtcNow,
                    model.Degree,
                    model.Stream,
                    model.WorkletComplexity,
                    model.DataCollection,
                    ImagePath = "",
                    TechDomainID = 0,
                    Skills = "",
                    Tags = "",
                    StatusID = 0,
                    Progress = "",
                    CertID = ""
                    // CertID = ""

                }, transaction: tran, commandType: CommandType.StoredProcedure);

                var secondaryMentors = model.Mentors.Where(m => !m.IsPrimary);
                foreach (var mentor in secondaryMentors)
                {
                    await conn.ExecuteAsync("PRISMWorklet_InsertSecondryMentor", new
                    {
                        WorkletID = workletId,
                        UID = mentor.MentorId,
                        IsActive = 1
                    }, transaction: tran, commandType: CommandType.StoredProcedure);
                }

                await tran.CommitAsync();
                return workletId;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<WorkletFullDetailsModel?> GetFullWorkletDetailsAsync(int initiatorMEmpId, int instanceId)
        {
            using var conn = new SqlConnection(_connectionString);
            var parameters = new { InitiatorMEmpID = initiatorMEmpId, InstanceID = instanceId };

            // Step 1: Fetch the main worklet details.
            var workletInfo = await conn.QueryFirstOrDefaultAsync<WorkletDetailsModel>(
                "PRISMWorklet_GetWorkletDetails",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (workletInfo == null)
            {
                return null; 
            }

            // Step 2: Fetch the mentor and attachment details.
            var mentors = await conn.QueryAsync<WorkletMentorDetailsModel>(
                "PRISMWorklet_GetMentorDetails",
                parameters,
                commandType:CommandType.StoredProcedure);

            var attachments = await conn.QueryAsync<AttachmentModel>(
                "PRISMWorklet_GetAttachmentDetails",
                parameters,
                commandType:CommandType.StoredProcedure);

            
            return new WorkletFullDetailsModel
            {
                WorkletInfo = workletInfo,
                Mentors = mentors,
                Attachments = attachments
            };
        }

        // public async Task<WorkletDetailsModel?> GetWorkletDetailsAsync(int initiatorMEmpID, int instanceID)
        // {
        //     using var conn = new SqlConnection(_connectionString);
        //     var parameters = new { InitiatorMEmpID = initiatorMEmpID, InstanceID = instanceID };
        //     return await conn.QueryFirstOrDefaultAsync<WorkletDetailsModel>(
        //         "PRISMWorklet_GetWorkletDetails", parameters, commandType: CommandType.StoredProcedure);
        // }

        // public async Task<IEnumerable<AttachmentModel>> GetAttachmentDetailsAsync(int initiatorMEmpID, int instanceID)
        // {
        //     using var conn = new SqlConnection(_connectionString);
        //     var parameters = new { InitiatorMEmpID = initiatorMEmpID, InstanceID = instanceID };
        //     return await conn.QueryAsync<AttachmentModel>(
        //         "PRISMWorklet_GetAttachmentDetails", parameters, commandType: CommandType.StoredProcedure);
        // }

        // public async Task<IEnumerable<WorkletMentorDetailsModel>> GetMentorDetailsAsync(int initiatorMEmpID, int instanceID)
        // {
        //     using var conn = new SqlConnection(_connectionString);
        //     var parameters = new { InitiatorMEmpID = initiatorMEmpID, InstanceID = instanceID };
        //     return await conn.QueryAsync<WorkletMentorDetailsModel>(
        //         "PRISMWorklet_GetMentorDetails", parameters, commandType: CommandType.StoredProcedure);
        // }


    }
}