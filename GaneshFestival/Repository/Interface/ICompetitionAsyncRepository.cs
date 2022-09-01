using GaneshFestival.Model;

namespace GaneshFestival.Repository.Interface
{
    public interface ICompetitionAsyncRepository
    {
        public Task<List<CompetitionNameModel>> GetCompetitionName();
        public Task<List<ZPGATModel>> GetZPGATName();
        public Task<long> AddCompetiton(CompetitionModel competition);
    }
}
