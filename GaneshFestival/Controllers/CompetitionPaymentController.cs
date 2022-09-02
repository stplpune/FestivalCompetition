using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using GaneshFestival.Repository.Interface;
using GaneshFestival.Model;

namespace GaneshFestival.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionPaymentController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICompetitionPaymentAsyncRepository CompetitionPaymentAsyncRepository;
        IConfiguration configuration;

        public CompetitionPaymentController(IConfiguration configuration, ILoggerFactory loggerFactory, ICompetitionPaymentAsyncRepository CompetitionPaymentAsyncRepository)
        {
            this.logger = loggerFactory.CreateLogger<LoginController>();
            this.CompetitionPaymentAsyncRepository = CompetitionPaymentAsyncRepository;
            this.configuration = configuration;
        }
        [HttpPost("save-update-Competition-payment")]
        public async Task<ActionResult> SaveUpdateVehiclePayment(VehiclePayment vehiclePayment)
        {
            logger.LogDebug(string.Format("CompetitionPaymentController-Post : Calling Adding  record"));
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var Execution = await CompetitionPaymentAsyncRepository.SaveUpdateVehiclePayment(vehiclePayment);

                var rtrMsg = string.Format("CompetitionPaymentController-SaveUpdateVehiclePayment : Completed ");
                logger.LogDebug(rtrMsg);
                responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                responseDetails.StatusMessage = rtrMsg;
                responseDetails.ResponseData = Execution;
                return Ok(responseDetails);

            }
            catch (Exception ex)
            {
                var msgStr = string.Format("Error in mahakhanijController-SaveUpdateVehiclePayment action with  Error:{0}", ex.Message.ToString());
                logger.LogInformation(msgStr);

                responseDetails.StatusCode = StatusCodes.Status409Conflict.ToString();
                responseDetails.StatusMessage = msgStr;
                return Ok(responseDetails);
            }

        }


        [HttpPost("generate-hash-sequence")]
        public async Task<ActionResult> generateHashSequence(HashModel hashModel)
        {
            logger.LogDebug(string.Format("CompetitionPaymentController-Post : Calling Adding  record"));
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var Execution = await CompetitionPaymentAsyncRepository.PaymentGateway_Hash(hashModel);

                var rtrMsg = string.Format("CompetitionPaymentController-generateHashSequence : Completed ");
                logger.LogDebug(rtrMsg);
                responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                responseDetails.StatusMessage = rtrMsg;
                responseDetails.ResponseData = Execution;
                return Ok(responseDetails);

            }
            catch (Exception ex)
            {
                var msgStr = string.Format("Error in CompetitionPaymentController-generateHashSequence action with  Error:{0}", ex.Message.ToString());
                logger.LogInformation(msgStr);

                responseDetails.StatusCode = StatusCodes.Status409Conflict.ToString();
                responseDetails.StatusMessage = msgStr;
                return Ok(responseDetails);
            }
        }

        [HttpPost("UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus(PaymentStatusModel payment)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                var ExtendTime = await CompetitionPaymentAsyncRepository.UpdatePaymentStatus(payment);
                if (ExtendTime > 0)
                {
                    var returnStr = string.Format("Payment Done.");
                    logger.LogInformation(returnStr);
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = ExtendTime;
                    return Ok(responseDetails);
                }
                else
                {
                    var msgStr = string.Format("Error...");
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