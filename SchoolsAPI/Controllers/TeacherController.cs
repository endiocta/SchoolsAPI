using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;
using SchoolsAPI.Repository;
using SchoolsAPI.Responses;

namespace SchoolsAPI.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherController : Controller
    {
        //private readonly TeacherRepository repo;
        private ApiDbContext db;
        private readonly ILogger<TeacherController> logger;
        //public TeacherController(TeacherRepository repo, ILogger<TeacherController> logger)
        //{
        //    this.repo = repo;
        //    this.logger = logger;
        //}
        public TeacherController(ApiDbContext db, ILogger<TeacherController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ResponseDto<List<Teacher>>> GetTeachersAsync()
        {
            ResponseDto<List<Teacher>> response = new ResponseDto<List<Teacher>>();
            try
            {
                //var result = await repo.GetAllTeachers();
                var result = await db.Teachers.ToListAsync();
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
                logger.LogError("Get All Teachers failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "error!";
            }
            return response;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ResponseDto<CommonResult>> AddAsync(Teacher teacher)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Teacher API Add, Data: {JsonConvert.SerializeObject(teacher)}");
                //var result = await repo.AddTeacher(teacher, logger);
                await db.Teachers.AddAsync(teacher);
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
                //    response.Result = result;
                //}
                CommonResult result = new CommonResult();
                result.message.Add("Teacher added successfully!");
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
