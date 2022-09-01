
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GaneshFestival.Model;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace GaneshFestival.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDocumentController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IHttpContextAccessor _contextAccessor;
        string uploaddocpath = "C:\\GaneshFestival\\Uploads\\";

        // string weburl = "";
        string weburl = "https://localhost:7280";

        private IHostingEnvironment _environment;
        IConfiguration configuration;
        public UploadDocumentController(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment environment, IHttpContextAccessor contextAccessor)
        {
            this.logger = loggerFactory.CreateLogger<UploadDocumentController>();
            this.configuration = configuration;
            _contextAccessor = contextAccessor;
            _environment = environment;
            uploaddocpath = configuration.GetSection("upload:uploaddocpath").Value;
            weburl = configuration.GetSection("BaseUrl:weburl").Value;
        }


        /// <summary>
        /// To Upload UploadFile.
        /// </summary>
        /// <param name="UploadFile"></param>
        /// <returns>message on success</returns>
        /// <response code="200">Successful Operation</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadFile([FromForm] DocumentModel document)
        {
            var request = _contextAccessor.HttpContext.Request;
            var domain = $"{request.Scheme}://{request.Host}";
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            string fileName = document.DocumentType.Replace("/", "_");
            var image = document.UploadDocPath;
            // Saving Image on Server
            try
            {
                string dir_path = uploaddocpath + document.FolderName;
                var uri = new System.Uri(dir_path);
                var converted = uri.AbsoluteUri;
                bool exists = System.IO.Directory.Exists(dir_path);
                if (!exists) { System.IO.Directory.CreateDirectory(dir_path); }
                if (image.Length > 0)
                {
                    string extension = Path.GetExtension(image.FileName);
                    string file = fileName + "_" + DateTime.Now.ToString("MMddyyyyhhmmsstt");

                    string path = Path.Combine(_environment.ContentRootPath, "Uploads/" + document.FolderName + "/" + file + extension);
                    path = Path.Combine(dir_path, file + extension);

                    string retPath1 = weburl + "/Uploads/" + document.FolderName + "/" + file + extension;
                    string imageName = image.FileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await document.UploadDocPath.CopyToAsync(stream);
                    }
                    string retPath = path.Replace((Path.Combine(_environment.ContentRootPath, "Uploads")), domain);
                    retPath = retPath1;
                    string returnStr = "File Posted Successfully";
                    logger.LogInformation(returnStr);
                    logger.LogDebug(string.Format("File Posted Successfully with DocumentType {0}.", document.DocumentType));
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = retPath;
                }
                return Ok(responseDetails);
            }
            catch (Exception ex)
            {
                var msgStr = string.Format(ex.ToString());
                logger.LogInformation(msgStr);
                responseDetails.StatusCode = StatusCodes.Status409Conflict.ToString();
                responseDetails.StatusMessage = msgStr;
                return Ok(responseDetails);
            }
        }

        /// <summary>
        /// To Upload MultipleFiles.
        /// </summary>
        /// <param name="UploadMultipleFiles"></param>
        /// <returns>message on success</returns>
        /// <response code="200">Successful Operation</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPost("UploadMultipleFiles")]
        public async Task<ActionResult> UploadMultipleFiles(string FolderName, [FromForm] string CommaSeparatedDocumentType, List<IFormFile> files)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            List<DocumentResponse> documentResponses = new List<DocumentResponse>();
            if (files.Count > 0)
            {

                string[] DocumentTypeList = CommaSeparatedDocumentType.Split(",");

                int fileCount = files.Count;
                int DocumentTypeCount = DocumentTypeList.Length;
                if (fileCount == DocumentTypeCount)
                {
                    var request = _contextAccessor.HttpContext.Request;
                    var domain = $"{request.Scheme}://{request.Host}";
                    int i = 0;
                    int j = 0;
                    foreach (var formFile in files)
                    {
                        i = i + 1;
                        DocumentResponse documentResponse = new DocumentResponse();
                        try
                        {
                            if (formFile.Length > 0)
                            {
                                string extension = Path.GetExtension(formFile.FileName);
                                string file = DateTime.Now.ToString("MMddyyyyhhmmsstt") + Convert.ToString(i);
                                //string path = Path.Combine(_environment.ContentRootPath, "Uploads/CorporatePlotFile/" + file + extension);
                                string path = Path.Combine(_environment.ContentRootPath, "Uploads/" + FolderName + "/" + file + extension);
                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                                string retPath = path.Replace((Path.Combine(_environment.ContentRootPath, "Uploads")), domain);

                                string ImageName = formFile.FileName;
                                //DocumentSaveModel documentSaveModel = new DocumentSaveModel();
                                //documentSaveModel.ApplicationNo = ApplicationNo;
                                //documentSaveModel.DocNo = ApplicationNo;
                                //documentSaveModel.DocType = DocumentTypeList[i - 1];
                                //documentSaveModel.DocPath = retPath;
                                //documentSaveModel.ImageName = ImageName;
                                //var Execution = await corporatePlotRegisterAsyncRepository.AddDocumentsAsync(documentSaveModel, logger);
                                string returnStr2 = "";
                                //if (Execution >= 1)
                                //{
                                //    returnStr2 = "File Posted Successfully";
                                //    documentResponse.StatusCode = StatusCodes.Status200OK.ToString();
                                //}
                                //else if (Execution == -2)
                                //{
                                //    var returnStrr = string.Format("Record is not added as there ApplicationNo {0} is not available.", documentSaveModel.ApplicationNo);
                                //    responseDetails.StatusCode = StatusCodes.Status409Conflict.ToString();
                                //    responseDetails.StatusMessage = returnStrr;
                                //    return Ok(responseDetails);
                                //}
                                //else
                                //{
                                //    returnStr2 = "Error while adding file";
                                //    documentResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                                //}
                                returnStr2 = "File Posted Successfully";
                                documentResponse.StatusCode = StatusCodes.Status200OK.ToString();
                                documentResponse.StatusMessage = returnStr2;
                                documentResponse.DocumentType = DocumentTypeList[j];
                                documentResponse.FilePath = retPath;
                                documentResponses.Add(documentResponse);

                            }
                            else
                            {
                                string returnStr1 = "File not found";
                                documentResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                                documentResponse.StatusMessage = returnStr1;
                                documentResponse.DocumentType = DocumentTypeList[j];
                                documentResponses.Add(documentResponse);
                            }
                        }
                        catch
                        {
                            var msgStr = string.Format("Error while adding files.");
                            logger.LogInformation(msgStr);
                            documentResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                            documentResponse.StatusMessage = msgStr;
                            return Ok(responseDetails);
                        }
                        j = j + 1;
                    }
                    string returnStr = "File Posted Successfully";
                    responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                    responseDetails.StatusMessage = returnStr;
                    responseDetails.ResponseData = documentResponses;
                    return Ok(responseDetails);
                }
                else
                {
                    var rtnStr = string.Format("List Items mismatched with CommaSeparatedDocumentType.");
                    logger.LogInformation(rtnStr);
                    responseDetails.StatusCode = StatusCodes.Status400BadRequest.ToString();
                    responseDetails.StatusMessage = rtnStr;
                    return Ok(responseDetails);
                }
            }
            else
            {
                var rtnStr = string.Format("Kindly enter valid record details.");
                logger.LogInformation(rtnStr);
                responseDetails.StatusCode = StatusCodes.Status400BadRequest.ToString();
                responseDetails.StatusMessage = rtnStr;
                return Ok(responseDetails);
            }
        }

        [HttpPost("UploadMultipleFiles_v1")]
        public async Task<ActionResult> UploadMultipleFiles_v1(string FolderName, List<IFormFile> files)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            List<DocumentResponse1> documentResponses = new List<DocumentResponse1>();
            if (files.Count > 0)
            {
                int fileCount = files.Count;
                var request = _contextAccessor.HttpContext.Request;
                var domain = $"{request.Scheme}://{request.Host}";
                int i = 0;
                int j = 0;
                foreach (var formFile in files)
                {
                    i = i + 1;
                    DocumentResponse1 documentResponse = new DocumentResponse1();
                    try
                    {
                        if (formFile.Length > 0)
                        {
                            string extension = Path.GetExtension(formFile.FileName);
                            string file = DateTime.Now.ToString("MMddyyyyhhmmsstt") + Convert.ToString(i);
                            //string path = Path.Combine(_environment.ContentRootPath, "Uploads/CorporatePlotFile/" + file + extension);
                            string path = Path.Combine(_environment.ContentRootPath, "Uploads/" + FolderName + "/" + file + extension);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            string retPath1 = weburl + "/Uploads/" + FolderName + "/" + file + extension;
                            //  string retPath = path.Replace((Path.Combine(_environment.ContentRootPath, "Uploads/")), domain);
                            string retPath = path.Replace((Path.Combine(_environment.ContentRootPath, "Uploads")), domain);
                            retPath = retPath1;
                            string ImageName = formFile.FileName;

                            string returnStr2 = "";

                            returnStr2 = "File Posted Successfully";
                            documentResponse.StatusCode = StatusCodes.Status200OK.ToString();
                            documentResponse.StatusMessage = returnStr2;
                            // documentResponse.DocumentType = "";
                            documentResponse.FilePath = retPath;
                            documentResponses.Add(documentResponse);

                        }
                        else
                        {
                            string returnStr1 = "File not found";
                            documentResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                            documentResponse.StatusMessage = returnStr1;
                            // documentResponse.DocumentType = "";
                            documentResponses.Add(documentResponse);
                        }
                    }
                    catch
                    {
                        var msgStr = string.Format("Error while adding files.");
                        logger.LogInformation(msgStr);
                        documentResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                        documentResponse.StatusMessage = msgStr;
                        return Ok(responseDetails);
                    }
                    j = j + 1;
                }
                string returnStr = "File Posted Successfully";
                responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                responseDetails.StatusMessage = returnStr;
                responseDetails.ResponseData = documentResponses;
                return Ok(responseDetails);
            }
            else
            {
                var rtnStr = string.Format("Kindly enter valid record details.");
                logger.LogInformation(rtnStr);
                responseDetails.StatusCode = StatusCodes.Status400BadRequest.ToString();
                responseDetails.StatusMessage = rtnStr;
                return Ok(responseDetails);
            }


        }
    }

}

