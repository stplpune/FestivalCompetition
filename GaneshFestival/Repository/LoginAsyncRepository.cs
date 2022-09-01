using Dapper;
using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
using System.Data.Common;

namespace GaneshFestival.Repository
{
    public class LoginAsyncRepository : BaseAsyncRepository, ILoginAsyncRepository
    {
        public LoginAsyncRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<LoginData> Login_1_0(string UserName,string Password)
        {
            LoginData competitions = new LoginData();

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"select Id,UserName from tblClientMaster where IsDeleted=0 and UserName=@UserName and Password=@Password";

                var comptition = await dbConnection.QueryAsync<LoginData>(query, new { UserName = UserName, Password = Password });
                competitions= comptition.FirstOrDefault() as LoginData;


            }
            return competitions;
        }

      
    }
}
