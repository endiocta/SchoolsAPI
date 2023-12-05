using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;
using SchoolsAPI.Responses;

namespace SchoolsAPI.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly ApiDbContext db;
        private readonly ILogger<CourseController> logger;
        public CourseController(ApiDbContext db, ILogger<CourseController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ResponseDto<List<Course>>> GetCoursesAsync()
        {
            ResponseDto<List<Course>> response = new ResponseDto<List<Course>>();
            try
            {
                var result = await db.Courses.ToListAsync();
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
                logger.LogError("Get All Courses failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "error!";
            }
            return response;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ResponseDto<CommonResult>> AddAsync(Course course)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Course API Add, Data: {JsonConvert.SerializeObject(course)}");
                await db.Courses.AddAsync(course);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Course added successfully!");
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

        [HttpPut("{id}")]
        public async Task<ResponseDto<CommonResult>> PutAsync(int id, Course course)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Course API Edit, Data: {JsonConvert.SerializeObject(course)}");
                if (id != course.CourseId)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Bad Request";
                    return response;
                }
                var oldData = db.Courses.Where(x => x.CourseId == id).FirstOrDefault();
                if (oldData == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Course not found!";
                    return response;
                }
                oldData.CourseName = course.CourseName;
                oldData.TeacherId = course.TeacherId;
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Course updated successfully!");
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
                var data = db.Courses.Where(x => x.CourseId == id).FirstOrDefault();
                if (data == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Data not found!";
                    return response;
                }
                db.Courses.Remove(data);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Course deleted successfully!");
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
