using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
namespace GaneshFestival.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ILoginAsyncRepository loginAsyncrepository;
        IConfiguration configuration;
        public LoginController(IConfiguration configuration, ILoggerFactory loggerFactory, ILoginAsyncRepository loginAsyncRepository)
        {
            this.logger = loggerFactory.CreateLogger<LoginController>();
            this.loginAsyncrepository = loginAsyncRepository;
            this.configuration = configuration;
        }
        [HttpGet("Login_1_0")]
        public async Task<ActionResult> Login_1_0(string UserName, string Password)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                logger.LogDebug(string.Format("Login_1_0- : Calling Login_1_0"));
                var durations = await loginAsyncrepository.Login_1_0(UserName, Password);

                if (durations.Id==0)
                {
                    var returnMsg = string.Format("Incorrect Username or Password.");
                    logger.LogInformation(returnMsg);
                    responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                    responseDetails.StatusMessage = returnMsg;
                    return Ok(responseDetails);
                }
                var rtrMsg = string.Format("Login successfully.");
                logger.LogDebug("Login_1_0");
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
