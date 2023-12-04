using Microsoft.EntityFrameworkCore;
using SchoolsAPI.DataAccess;
using SchoolsAPI.Models;

namespace SchoolsAPI.Repository
{
    public class StandardRepository
    {
        private readonly ApiDbContext db;

        public StandardRepository(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Standard>> GetAllStandards()
        {
            List<Standard> result = new List<Standard>();
            var standards = await db.Standards.ToListAsync();
            if (standards != null && standards.Count > 0)
            {
                foreach ( var standard in standards)
                {
                    result.Add(standard);
                }
            }
            return result;
        }

        public async Task<ResultStandard> InsertNewStandard(Standard standard, ILogger logger)
        {
            ResultStandard result = new ResultStandard();
            try
            {
                await db.Standards.AddAsync(standard);
                await db.SaveChangesAsync();
                result.message.Add("Standard added successfully!");
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
        public class ResultStandard
        {
            public List<string> message { get; set; } = new List<string>();
        }
    }
}
