using GaneshFestival.Model;

namespace GaneshFestival.Repository.Interface
{
    public interface ICompetitionAsyncRepository
    {
        public Task<List<CompetitionModel>> GetCompetitionName();
        public Task<List<ZPGATModel>> GetZPGATName();
    }
}
