using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;
using SchoolsAPI.Repository;
using SchoolsAPI.Responses;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SchoolsAPI.Controllers
{
    [Route("api/Standard")]
    [ApiController]
    public class StandardController : Controller
    {
        //private readonly StandardRepository repo;
        private ApiDbContext db;
        private readonly ILogger<StandardController> logger;
        //public StandardController(StandardRepository repo, ILogger<StandardController> logger)
        //{
        //    this.repo = repo;
        //    this.logger = logger;
        //}
        public StandardController(ApiDbContext db, ILogger<StandardController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ResponseDto<List<Standard>>> GetStandardsAsync()
        {
            ResponseDto<List<Standard>> response = new ResponseDto<List<Standard>>();
            try
            {
                //var result = await repo.GetAllStandards();
                var result = await db.Standards.ToListAsync();
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "No data!";
                }
                else
                    response.Result = result;
            }
            catch (Exception ex)
            {
                logger.LogError("Get All Standards failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "error!";
            }
            return response;
        }

        [HttpPost]
        public async Task<ResponseDto<StandardRepository.ResultStandard>> SendAsync(Standard standard)
        {
            ResponseDto<StandardRepository.ResultStandard> response = new ResponseDto<StandardRepository.ResultStandard>();
            try
            {
                logger.LogInformation($"Standard API Send, Data: {JsonConvert.SerializeObject(standard)}");
                //var result = await repo.InsertNewStandard(standard, logger);
                await db.Standards.AddAsync(standard);
                await db.SaveChangesAsync();
                //var validResult = result.message.Where(x => x.Contains("Failed")).ToList();
                //if (validResult.Count() > 0)
                //{
                //    response.IsSuccess = false;
                //    response.ErrorMessages = new List<string>();
                //    response.ErrorMessages = result.message;
                //    response.DisplayMessage = "Save Failed!";
                //}
                //else
                //{
                //    StandardRepository.ResultStandard result = new StandardRepository.ResultStandard();
                //    result.message.Add("Standard added successfully!");
                //    response.Result = result;
                //}
                StandardRepository.ResultStandard result = new StandardRepository.ResultStandard();
                result.message.Add("Standard added successfully!");
                response.Result = result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Save Failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "Save Failed!";
            }
            return response;
        }
    }
}