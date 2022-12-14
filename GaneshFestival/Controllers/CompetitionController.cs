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

        [HttpGet("GetCompetitionData")]
        public async Task<ActionResult> GetCompetitionData(int CompetitionTypeId,
            int ZPGATId, string? SearhchText, int ClientId, int PageNo)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var jobpostdetails = await competitionAsyncRepository.GetCompetitionData(CompetitionTypeId, ZPGATId,
                    SearhchText, ClientId, PageNo);

                if (jobpostdetails != null)
                {
                    var returnStr = string.Format("Data fetch Succesfully..");
                    logger.LogInformation(returnStr);
                    logger.LogDebug(string.Format("CompetitionController-Create : Completed Adding record with Id."));
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = jobpostdetails;
                    return Ok(responseDetails);
                }
                else
                {
                    var msgStr = string.Format("Error while getting record.");
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


        [HttpGet("GetOtherCompetitionData")]
        public async Task<ActionResult> GetOtherCompetitionData(int CompetitionId)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var jobpostdetails = await competitionAsyncRepository.GetOtherCompetitionData(CompetitionId);

                if (jobpostdetails != null)
                {
                    var returnStr = string.Format("Data fetch Succesfully..");
                    logger.LogInformation(returnStr);
                    logger.LogDebug(string.Format("CompetitionController-Create : Completed Adding record with Id."));
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = jobpostdetails;
                    return Ok(responseDetails);
                }
                else
                {
                    var msgStr = string.Format("Error while getting record.");
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

        [HttpGet("GetDashboardCount")]
        public async Task<ActionResult> GetDashboardCount(int ClientId)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var jobpostdetails = await competitionAsyncRepository.GetDashboardCount(ClientId);

                if (jobpostdetails != null)
                {
                    var returnStr = string.Format("Data fetch Succesfully..");
                    logger.LogInformation(returnStr);
                    logger.LogDebug(string.Format("CompetitionController-Create : Completed Adding record with Id."));
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = jobpostdetails;
                    return Ok(responseDetails);
                }
                else
                {
                    var msgStr = string.Format("Error while getting record.");
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

        [HttpPost("UpdateMarks")]
        public async Task<ActionResult> UpdateMarks(long CompetitionId, decimal Marks, string Remark)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var returndata = await competitionAsyncRepository.UpdateMarks(CompetitionId, Marks, Remark);

                if (returndata > 0)
                {
                    var returnStr = string.Format("Data Saved Succesfully..");
                    logger.LogInformation(returnStr);
                    logger.LogDebug(string.Format("CompetitionController-Create : Completed Adding record with Id."));
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = "";
                    return Ok(responseDetails);
                }
                else
                {
                    var msgStr = string.Format("Error while getting record.");
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
