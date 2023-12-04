using Microsoft.EntityFrameworkCore;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;

namespace SchoolsAPI.Repository
{
    public class TeacherRepository
    {
        private readonly ApiDbContext db;

        public TeacherRepository(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Teacher>> GetAllTeachers()
        {
            List<Teacher> result = new List<Teacher>();
            var teachers = await db.Teachers.ToListAsync();
            if (teachers != null)
            {
                foreach (var teacher in teachers)
                {
                    result.Add(teacher);
                }
            }
            return result;
        }

        public async Task<ResultTeacher> AddTeacher(Teacher teacher, ILogger logger)
        {
            ResultTeacher result = new ResultTeacher();
            try
            {
                await db.Teachers.AddAsync(teacher);
                await db.SaveChangesAsync();
                result.message.Add("Teacher added successfully!");
            }
            catch (Exception ex)
            {
                logger?.LogError($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    logger?.LogError($"Inner Error: {ex.InnerException.Message}");
                }
                logger?.LogError($"Stack Trace: {ex.StackTrace}");
                result.message.Add("Failed, Can't Add Standard!");
            }
            return result;
        }

        public class ResultTeacher
        {
            public List<string> message { get; set; } = new List<string>();
        }
    }
}
