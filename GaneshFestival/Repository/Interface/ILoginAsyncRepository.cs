using GaneshFestival.Model;

namespace GaneshFestival.Repository.Interface
{
    public interface ILoginAsyncRepository
    {
        public Task<LoginData> Login_1_0(string UserName,string Password);
   
    }
}
