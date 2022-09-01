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
            List<CompetitionModel> competitions = new List<CompetitionModel>();

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"select Id,CompetitionName from tblCompetitionType where IsDeleted=0";
                var comptition = await dbConnection.QueryAsync<CompetitionModel>(query);
                competitions = comptition.ToList();
            }
            return competitions;
        }

        public async Task<List<ZPGATModel>> GetZPGATName()
        {
            List<ZPGATModel> zPGATModels = new List<ZPGATModel>();

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"select Id,ZPGATName,ClientId from tblZPGATMaster where IsDeleted=0";
                var zPGATs = await dbConnection.QueryAsync<ZPGATModel>(query);
                zPGATModels = zPGATs.ToList();
            }
            return zPGATModels;

        }
    }
}
