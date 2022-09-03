using GaneshFestival.Model;

namespace GaneshFestival.Repository.Interface
{
    public interface ICompetitionAsyncRepository
    {
        public Task<List<CompetitionNameModel>> GetCompetitionName();
        public Task<List<ZPGATModel>> GetZPGATName();
        public Task<long> AddCompetiton(CompetitionModel competition);
        Task<long> AddCompettionImages(long id, List<CompettionImages> compettionImages);

        public Task<List<clsCompetitionData>> GetCompetitionData(int CompetitionTypeId,
            int ZPGATId,string? SearhchTex,int ClientId,int PageNo);

        public Task<clsCompetitionOtherData> GetOtherCompetitionData(long CompetitionId);
        public Task<List<clsDashboardData>> GetDashboardCount(int ClientId);
        public Task<long> UpdateMarks(long CompetitionId,decimal Marks,string Remark);



    }
}
