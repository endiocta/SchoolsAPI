using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;
using SchoolsAPI.Responses;

namespace SchoolsAPI.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly ApiDbContext db;
        private readonly ILogger<StudentController> logger;
        public StudentController(ApiDbContext db, ILogger<StudentController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ResponseDto<List<Student>>> GetStudentsAsync()
        {
            ResponseDto<List<Student>> response = new ResponseDto<List<Student>>();
            try
            {
                var result = await db.Students.ToListAsync();
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
                logger.LogError("Get All Students failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "error!";
            }
            return response;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ResponseDto<CommonResult>> AddAsync(Student student)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Student API Add, Data: {JsonConvert.SerializeObject(student)}");
                await db.Students.AddAsync(student);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Student added successfully!");
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
        public async Task<ResponseDto<CommonResult>> PutAsync(int id, Student student)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Student API Edit, Data: {JsonConvert.SerializeObject(student)}");
                if (id != student.StudentId)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Bad Request";
                    return response;
                }
                var oldData = db.Students.Where(x => x.StudentId == id).FirstOrDefault();
                if (oldData == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Student not found!";
                    return response;
                }
                oldData.BirthPlace = student.BirthPlace;
                oldData.BirthDate = student.BirthDate;
                oldData.FirstName = student.FirstName;
                oldData.LastName = student.LastName;
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Student updated successfully!");
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
                var data = db.Students.Where(x => x.StudentId == id).FirstOrDefault();
                if (data == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Data not found!";
                    return response;
                }
                db.Students.Remove(data);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Student deleted successfully!");
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
