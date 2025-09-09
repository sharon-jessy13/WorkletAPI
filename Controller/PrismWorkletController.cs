using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Drawing;
using System.Text;
using PrismWorkletApi.Repositories;
using PrismWorkletApi.Models;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class PrismWorkletController : ControllerBase
{
    private readonly IWorkletRepository _workletRepository;
    private readonly IMentorRepository _mentorRepository;
    private readonly ILogger<PrismWorkletController> _logger;

    public PrismWorkletController(
        IWorkletRepository workletRepository,
        IMentorRepository mentorRepository,
        ILogger<PrismWorkletController> logger)
    {
        _workletRepository = workletRepository;
        _mentorRepository = mentorRepository;
        _logger = logger;
    }

    [HttpPost("upload-ppt")]
    public async Task<IActionResult> UploadPpt(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
        if (System.IO.Path.GetExtension(file.FileName).ToLowerInvariant() != ".pptx") return BadRequest("Invalid file type.");

        try
        {
            var data = new PptExtractedData();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (PresentationDocument doc = PresentationDocument.Open(stream, false))
                {
                    var slideParts = doc.PresentationPart?.SlideParts.ToList();
                    if (slideParts == null || !slideParts.Any()) return Ok(data);

                    var firstSlideText = slideParts.First().Slide.Descendants<Text>().Select(t => t.Text);
                    data.Title = firstSlideText.FirstOrDefault() ?? "Title not found";

                    var allTextBuilder = new StringBuilder();
                    foreach (var slidePart in slideParts)
                    {
                        if (slidePart.Slide != null)
                        {
                            foreach (var text in slidePart.Slide.Descendants<Text>())
                            {
                                allTextBuilder.Append(text.Text + " ");
                            }
                        }
                    }
                    string fullText = allTextBuilder.ToString();

                    const string problemStmtKeyword = "Problem Statement";
                    int problemStmtIndex = fullText.IndexOf(problemStmtKeyword, StringComparison.OrdinalIgnoreCase);
                    if (problemStmtIndex != -1)
                    {
                        int startIndex = problemStmtIndex + problemStmtKeyword.Length;
                        int length = Math.Min(500, fullText.Length - startIndex);
                        data.ProblemStatement = fullText.Substring(startIndex, length).Trim();
                    }
                    data.Prerequisites = fullText;
                }
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract data from PPTX file: {FileName}", file.FileName);
            return StatusCode(500, "Failed to extract data. Please fill the fields manually.");
        }
    }

    // [HttpGet("colleges")]
    // public async Task<IActionResult> GetColleges([FromQuery] int initiatorMEmpId, [FromQuery] int instanceId)
    // {
    //     if (initiatorMEmpId <= 0 || instanceId <= 0)
    //     {
    //         return BadRequest("Initiator employee ID and instance ID are required.");
    //     }

    //     var colleges = await _mentorRepository.GetCollegesAsync(initiatorMEmpId, instanceId);
    //     return Ok(colleges);
    // }

    [HttpGet("mentors/search")]
    public async Task<IActionResult> SearchMentors([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(Enumerable.Empty<MentorSearchResult>());
        }
        var mentors = await _mentorRepository.SearchAsync(query);
        return Ok(mentors);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> CreateWorklet([FromBody] WorkletCreateModel model)
    {
        if (model == null)
        {
            return BadRequest("Invalid worklet data provided.");
        }

        if (!model.Mentors.Any(m => m.IsPrimary))
        {
            return BadRequest("A primary mentor must be specified.");
        }

        if (string.IsNullOrWhiteSpace(model.Title) ||
            string.IsNullOrWhiteSpace(model.ProblemStatement) ||
            string.IsNullOrWhiteSpace(model.Prerequisites))
        {
            return BadRequest("Title, Problem Statement, and Prerequisites are mandatory fields.");
        }


        try
        {
            var workletId = await _workletRepository.CreateWorkletAsync(model);
            return Ok(new { WorkletID = workletId, Message = "Worklet created successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the worklet for title: {WorkletTitle}", model.Title);
            return StatusCode(500, "An error occurred while creating the worklet.");
        }
    }

    [HttpGet("{instanceId}/details")]
    public async Task<IActionResult> GetWorkletDetails(int instanceId, [FromQuery] int initiatorMEmpId)
    {
        if (initiatorMEmpId <= 0) return BadRequest("A valid initiator employee ID is required.");
        var details = await _workletRepository.GetWorkletDetailsAsync(initiatorMEmpId, instanceId);
        if (details == null) return NotFound("Worklet details not found.");
        return Ok(details);
    }

    [HttpGet("{instanceId}/attachments")]
    public async Task<IActionResult> GetAttachmentDetails(int instanceId, [FromQuery] int initiatorMEmpId)
    {
        if (initiatorMEmpId <= 0) return BadRequest("A valid initiator employee ID is required.");
        var attachments = await _workletRepository.GetAttachmentDetailsAsync(initiatorMEmpId, instanceId);
        return Ok(attachments);
    }

    [HttpGet("{instanceId}/mentors")]
    public async Task<IActionResult> GetMentorDetails(int instanceId, [FromQuery] int initiatorMEmpId)
    {
        if (initiatorMEmpId <= 0) return BadRequest("A valid initiator employee ID is required.");
        var mentors = await _workletRepository.GetMentorDetailsAsync(initiatorMEmpId, instanceId);
        return Ok(mentors);
    }
}