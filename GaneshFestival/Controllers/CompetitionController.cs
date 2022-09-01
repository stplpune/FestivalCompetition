using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
namespace GaneshFestival.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICompetitionAsyncRepository competitionAsyncRepository;
        IConfiguration configuration;
        public CompetitionController(IConfiguration configuration, ILoggerFactory loggerFactory, ICompetitionAsyncRepository competitionAsyncRepository)
        {
            this.logger = loggerFactory.CreateLogger<CompetitionController>();
            this.competitionAsyncRepository = competitionAsyncRepository;
            this.configuration = configuration;
        }
        [HttpGet("GetCompetitionName")]
        public async Task<ActionResult> GetCompetitionName()
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                logger.LogDebug(string.Format("CompetitionController-getAll : Calling getAll"));
                var durations = await competitionAsyncRepository.GetCompetitionName();

                if (durations.Count == 0)
                {
                    var returnMsg = string.Format("There are not any Competition.");
                    logger.LogInformation(returnMsg);
                    responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                    responseDetails.StatusMessage = returnMsg;
                    return Ok(responseDetails);
                }
                var rtrMsg = string.Format("All  records are fetched successfully.");
                logger.LogDebug("CompetitionController-getAll : Completed Get action all getAll records.");
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

        [HttpGet("GetZPGATName")]
        public async Task<ActionResult> GetZPGATName()
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                logger.LogDebug(string.Format("CompetitionController-getAll : Calling GetZPGATName"));
                var durations = await competitionAsyncRepository.GetZPGATName();

                if (durations.Count == 0)
                {
                    var returnMsg = string.Format("There are not any ZPGAT Available.");
                    logger.LogInformation(returnMsg);
                    responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                    responseDetails.StatusMessage = returnMsg;
                    return Ok(responseDetails);
                }
                var rtrMsg = string.Format("All  records are fetched successfully.");
                logger.LogDebug("CompetitionController-getAll : Completed Get action all getAll records.");
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
        [HttpPost]
        public async Task<IActionResult> AddCompetiton(CompetitionModel competition)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var jobpostdetails = await competitionAsyncRepository.AddCompetiton(competition);
                if (jobpostdetails > 0)
                {
                    var returnStr = string.Format("Data added Succesfully..");
                    logger.LogInformation(returnStr);
                    logger.LogDebug(string.Format("CompetitionController-Create : Completed Adding record with Id."));
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = jobpostdetails;
                    return Ok(responseDetails);
                }
                else
                {
                    var msgStr = string.Format("Error while adding record.");
                    logger.LogInformation(msgStr);
                    responseDetails.StatusCode = StatusCodes.Status409Conflict.ToString();
                    responseDetails.StatusMessage = msgStr;
                    return Ok(responseDetails);
                }
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
        }


    }
}
