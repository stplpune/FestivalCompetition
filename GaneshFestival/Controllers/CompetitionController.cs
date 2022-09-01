using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaneshFestival.Controllers
{
    [Route("GaneshFestival/Competition")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICompetitionAsyncRepository CompetitionAsyncRepository;
        IConfiguration configuration;
        public CompetitionController(IConfiguration configuration, ILoggerFactory loggerFactory, ICompetitionAsyncRepository masterAsyncRepository)
        {
            this.logger = loggerFactory.CreateLogger<CompetitionController>();
            this.CompetitionAsyncRepository = CompetitionAsyncRepository;
            this.configuration = configuration;
        }
        [HttpGet("GetAllEmployement")]
        public async Task<ActionResult> GetAllEmployement()
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                logger.LogDebug(string.Format("CompetitionController-getAll : Calling getAll"));
                var durations = await CompetitionAsyncRepository.GetCompetitionName();

                if (durations.Count == 0)
                {
                    var returnMsg = string.Format("There are not any Competition.");
                    logger.LogInformation(returnMsg);
                    responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                    responseDetails.StatusMessage = returnMsg;
                    return Ok(responseDetails);
                }
                var rtrMsg = string.Format("All  records are fetched successfully.");
                logger.LogDebug("MasterController-getAll : Completed Get action all getAll records.");
                responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                responseDetails.StatusMessage = rtrMsg;
                responseDetails.ResponseData = durations;
            }
            catch (Exception ex)
            {
                //log error
                logger.LogError(ex.Message);
                var returnMsg = string.Format(ex.Message);
                logger.LogInformation(returnMsg);
                responseDetails.StatusCode = StatusCodes.Status409Conflict.ToString();
                responseDetails.StatusMessage = returnMsg;
                return Ok(responseDetails);
            }
            return Ok(responseDetails);
        }
    }
}
