﻿using FileAnalysisService.Data;
using FileAnalysisService.DTOs; 
using FileAnalysisService.Models;
using FileAnalysisService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FileAnalysisService.Controllers
{
    [Route("api/analysis")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly IFileAnalysisOrchestratorService _analysisOrchestratorService;
        private readonly FileAnalysisDbContext _dbContext;
        private readonly ILogger<AnalysisController> _logger;
        private readonly FileAnalysisService.Services.IFileStorageProvider _localFileStorageProvider; 

        public AnalysisController(
            IFileAnalysisOrchestratorService analysisOrchestratorService,
            FileAnalysisDbContext dbContext,
            FileAnalysisService.Services.IFileStorageProvider localFileStorageProvider, 
            ILogger<AnalysisController> logger)
        {
            _analysisOrchestratorService = analysisOrchestratorService;
            _dbContext = dbContext;
            _localFileStorageProvider = localFileStorageProvider;
            _logger = logger;
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestAnalysis([FromBody] AnalysisRequestDto request)
        {
            if (request == null || request.FileId == Guid.Empty || string.IsNullOrEmpty(request.FileHash))
            {
                _logger.LogWarning("RequestAnalysis called with invalid parameters. FileId: {FileId}, FileHash: {FileHash}", request?.FileId, request?.FileHash);
                return BadRequest("FileId and FileHash are required.");
            }

            _logger.LogInformation("Analysis requested for FileId: {FileId}, FileHash: {FileHash}, ForceReanalyze: {ForceReanalyze}",
                request.FileId, request.FileHash, request.ForceReanalyze);

            try
            {
                if (!request.ForceReanalyze) 
                {
                    var existingCompletedResult = await _dbContext.AnalysisResults
                                                          .AsNoTracking()
                                                          .FirstOrDefaultAsync(r => r.FileId == request.FileId && r.Status == AnalysisStatus.Completed);

                    if (existingCompletedResult != null)
                    {
                        _logger.LogInformation("Returning already completed analysis for FileId: {FileId} as ForceReanalyze is false.", request.FileId);
                        return Ok(MapToDto(existingCompletedResult));
                    }
                }
                else 
                {
                    _logger.LogInformation("ForceReanalyze is true for FileId: {FileId}. Proceeding with new analysis or update.", request.FileId);
                    var oldResult = await _dbContext.AnalysisResults.FirstOrDefaultAsync(r => r.FileId == request.FileId);
                    if (oldResult != null) { _dbContext.AnalysisResults.Remove(oldResult); await _dbContext.SaveChangesAsync(); }
                }
                
                var analysisResult = await _analysisOrchestratorService.AnalyzeFileAsync(request.FileId, request.FileHash);
                var dtoResult = MapToDto(analysisResult);

                if (analysisResult.Status == AnalysisStatus.Completed)
                {
                    return Ok(dtoResult);
                }
                else if (analysisResult.Status == AnalysisStatus.InProgress || analysisResult.Status == AnalysisStatus.Pending)
                {
                    return AcceptedAtAction(nameof(GetAnalysisResultByFileId), new { fileId = analysisResult.FileId }, dtoResult);
                }
                else 
                {
                    _logger.LogError("Analysis failed for FileId: {FileId}. Error: {ErrorMessage}", request.FileId, analysisResult.ErrorMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, dtoResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during RequestAnalysis for FileId: {FileId}", request.FileId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        [HttpGet("file/{fileId}")]
        [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnalysisResultByFileId(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                return BadRequest("FileId is required.");
            }

            _logger.LogInformation("Fetching analysis result for FileId: {FileId}", fileId);
            var analysisResult = await _dbContext.AnalysisResults
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.FileId == fileId);

            if (analysisResult == null)
            {
                _logger.LogWarning("Analysis result not found for FileId: {FileId}", fileId);
                return NotFound($"No analysis result found for FileId {fileId}.");
            }

            return Ok(MapToDto(analysisResult));
        }
        
        [HttpGet("{analysisId}")]
        [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnalysisResultById(Guid analysisId)
        {
            if (analysisId == Guid.Empty)
            {
                return BadRequest("AnalysisId is required.");
            }
            _logger.LogInformation("Fetching analysis result for AnalysisId: {AnalysisId}", analysisId);
            var analysisResult = await _dbContext.AnalysisResults
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == analysisId);
            if (analysisResult == null)
            {
                _logger.LogWarning("Analysis result not found for AnalysisId: {AnalysisId}", analysisId);
                return NotFound($"No analysis result found for AnalysisId {analysisId}.");
            }
            return Ok(MapToDto(analysisResult));
        }


        [HttpGet("wordcloud/file/{fileId}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        private AnalysisResultDto MapToDto(AnalysisResult result)
        {
            return new AnalysisResultDto
            {
                AnalysisId = result.Id,
                FileId = result.FileId,
                FileContentHash = result.FileContentHash,
                ParagraphCount = result.ParagraphCount,
                WordCount = result.WordCount,
                CharCount = result.CharCount,
                PlagiarismScores = result.PlagiarismScores,
                WordCloudImageLocation = result.WordCloudImageLocation,
                Status = result.Status.ToString(),
                RequestedAt = result.RequestedAt,
                CompletedAt = result.CompletedAt,
                ErrorMessage = result.ErrorMessage
            };
        }
    }
}