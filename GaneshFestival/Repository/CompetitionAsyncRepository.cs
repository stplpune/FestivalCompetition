using Dapper;
using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
using System.Data.Common;

namespace GaneshFestival.Repository
{
    public class CompetitionAsyncRepository : BaseAsyncRepository, ICompetitionAsyncRepository
    {
        public CompetitionAsyncRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<CompetitionModel>> GetCompetitionName()
        {
            List<CompetitionModel> employmentModels = new List<CompetitionModel>();

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"select Id,CompetitionName from tblCompetitionType where IsDeleted=0";
                var employment = await dbConnection.QueryAsync<CompetitionModel>(query);
                employmentModels = employment.ToList();
            }
            return employmentModels;
        }
       
    }
}
