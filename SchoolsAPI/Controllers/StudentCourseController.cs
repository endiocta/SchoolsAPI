using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;
using SchoolsAPI.Responses;

namespace SchoolsAPI.Controllers
{
    [Route("api/StudentCourse")]
    [ApiController]
    public class StudentCourseController : Controller
    {
        private readonly ApiDbContext db;
        private readonly ILogger<StudentCourseController> logger;
        public StudentCourseController(ApiDbContext db, ILogger<StudentCourseController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ResponseDto<List<StudentCourse>>> GetStudentCoursesAsync()
        {
            ResponseDto<List<StudentCourse>> response = new ResponseDto<List<StudentCourse>>();
            try
            {
                var result = await db.StudentCourses.ToListAsync();
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "No data.";
                }
                else
                    response.Result = result;
            }
            catch (Exception ex)
            {
                logger.LogError("Get All Student Courses failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "error!";
            }
            return response;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ResponseDto<CommonResult>> AddAsync(StudentCourse studentCourse)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"StudentCourse API Add, Data: {JsonConvert.SerializeObject(studentCourse)}");
                await db.StudentCourses.AddAsync(studentCourse);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("StudentCourse added successfully!");
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

        [HttpDelete("{id}")]
        public async Task<ResponseDto<CommonResult>> DeleteAsync(int id)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                var data = db.StudentCourses.Where(x => x.StudentCourseId == id).FirstOrDefault();
                if (data == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Data not found!";
                    return response;
                }
                db.StudentCourses.Remove(data);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("StudentCourse deleted successfully!");
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
