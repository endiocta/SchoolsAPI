using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;
using SchoolsAPI.Responses;

namespace SchoolsAPI.Controllers
{
    [Route("api/StudentAddress")]
    [ApiController]
    public class StudentAddressController : Controller
    {
        private readonly ApiDbContext db;
        private readonly ILogger<StudentAddressController> logger;
        public StudentAddressController(ApiDbContext db, ILogger<StudentAddressController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        //[HttpGet("{studentid}")]
        [HttpGet]
        [Route("Get")]
        public async Task<ResponseDto<StudentAddress>> GetByStudentIdAsync(int studentid)
        {
            ResponseDto<StudentAddress> response = new ResponseDto<StudentAddress>();
            try
            {
                var result = await db.StudentAddresses.Where(x => x.StudentId == studentid).FirstOrDefaultAsync();
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Address cannot be found!";
                }
                else
                    response.Result = result;
            }
            catch (Exception ex)
            {
                logger.LogError("Get Student's Address failed!", ex);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>();
                response.ErrorMessages.Add(ex.Message);
                response.DisplayMessage = "error!";
            }
            return response;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ResponseDto<CommonResult>> AddAsync(StudentAddress address)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Student Address API Add, Data: {JsonConvert.SerializeObject(address)}");
                await db.StudentAddresses.AddAsync(address);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Student Address added successfully!");
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
        public async Task<ResponseDto<CommonResult>> PutAsync(int id, StudentAddress address)
        {
            ResponseDto<CommonResult> response = new ResponseDto<CommonResult>();
            try
            {
                logger.LogInformation($"Student API Edit, Data: {JsonConvert.SerializeObject(address)}");
                if (id != address.StudentAddressId)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Bad Request";
                    return response;
                }
                var oldData = db.StudentAddresses.Where(x => x.StudentAddressId == id).FirstOrDefault();
                if (oldData == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Student Address not found!";
                    return response;
                }
                oldData.Address1 = address.Address1;
                oldData.Address2 = address.Address2;
                oldData.City = address.City;
                oldData.State = address.State;
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Student Address updated successfully!");
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
                var data = db.StudentAddresses.Where(x => x.StudentAddressId == id).FirstOrDefault();
                if (data == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Data not found!";
                    return response;
                }
                db.StudentAddresses.Remove(data);
                await db.SaveChangesAsync();
                CommonResult result = new CommonResult();
                result.message.Add("Student Address deleted successfully!");
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
