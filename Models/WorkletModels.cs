namespace PrismWorkletApi.Models
{
    public class WorkletCreateModel
    {
        // Basic Details
        public string Title { get; set; }
        public string ProblemStatement { get; set; }
        public string Prerequisites { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StudentCount { get; set; }
        public int InitiatorMEmpId { get; set; }

        // Mentor Details
        public List<MentorModel> Mentors { get; set; } = new List<MentorModel>();

        // Additional Fields from the document
        public string? GitHubUrl { get; set; }
        public string? Degree { get; set; }
        public string? Stream { get; set; }
        public string? WorkletComplexity { get; set; }
        public string? Research { get; set; }
        public bool POC { get; set; }
        public string? DataCollection { get; set; }
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
        public int EmployeeId { get; set; }
        public string Name { get; set; }
    }

    public class WorkletDetailsModel
    {
        public int WorkletID { get; set; }
        public string? Title { get; set; }
        public string? ProblemStatement { get; set; }
        public string? Prerequisites { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StudentCount { get; set; }
        public string? CreatedMentor { get; set; }
        public string? GitHubUrl { get; set; }
    }

    public class AttachmentModel
    {
        public int AttachmentID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
    }

    public class WorkletMentorDetailsModel : MentorModel
    {
        // Inherits from MentorModel
    }
}