namespace PrismWorkletApi.Models
{
    public class WorkletCreateModel
{
    public string Title { get; set; } = string.Empty;
    public string ProblemStatement { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int StudentCount { get; set; }
    public int InitiatorMEmpId { get; set; }
    public List<MentorModel> Mentors { get; set; } = new();
    public string? GitHubUrl { get; set; }
    public int Degree { get; set; }
    public int Stream { get; set; }
    public int WorkletComplexity { get; set; }
    public bool DataCollection { get; set; }

    public bool Research { get; set; }
    public bool POC { get; set; }
    public bool IsLinkedProject { get; set; }
    public int? ProjectID { get; set; }
    public int DomainID { get; set; }
    public string? OtherDomain { get; set; }
}
    public class MentorModel
    {
        public int MentorId { get; set; }
        public string MentorName { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class PptExtractedData
    {
        public string? Title { get; set; }
        public string? ProblemStatement { get; set; }
        public string? Prerequisites { get; set; }
    }

    public class MentorSearchResult
    {
       public int MEmpID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int EId { get; set; }
    }

    public class WorkletDetailsModel
    {
        public int WorkletID { get; set; }
        public string? Title { get; set; }
        public string? ImagePath { get; set; }
        public string? ProblemStmt { get; set; }
        public string? Prerequest { get; set; }
        public int ProjectID { get; set; }
        public string? GitHubUrl { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedMentorID { get; set; }
        public string? CreatedMentor { get; set; }
        public string? Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StudentCount { get; set; }
        public int Degree { get; set; }
        public int Stream { get; set; }
        public int WorkletComplexity { get; set; }
    }

    public class AttachmentModel
    {
        public int AttachmentID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
    }

    public class WorkletFullDetailsModel
    {
        public WorkletDetailsModel? WorkletInfo { get; set; }
        public IEnumerable<WorkletMentorDetailsModel>? Mentors { get; set; }
        public IEnumerable<AttachmentModel>? Attachments { get; set; }
    }

    
    
    // public class CollegeModel
    // {
    //     public int CollegeId { get; set; }
    //     public string CollegeName { get; set; } = string.Empty;
    // }

    public class WorkletMentorDetailsModel : MentorModel
    {

    }
}