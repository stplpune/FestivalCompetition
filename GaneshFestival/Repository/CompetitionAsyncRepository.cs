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

        public async Task<long> AddCompetiton(CompetitionModel competition)
        {
            competition.CreatedBy = 1;
            competition.CreatedDate = DateTime.Now;
            competition.IsDeleted = false;

            int result = 0;
            string str_skill = "";
            string str_qua = "";         

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                   

                     var query = @"Insert into tblcompetition (CompetitionTypeId,ZPGATId,ClientId, VillageName,PersonName,MobileNo,Amount,   
                                          PaymentScreenPath,VideoPath,PaymentId,PaymentStatus,Remark,Marks,MoreInfo,CreatedBy,CreatedDate,IsDeleted)
                                          values(@CompetitionTypeId,@ZPGATId,@ClientId, @VillageName,@PersonName,@MobileNo,@Amount,@PaymentScreenPath,@VideoPath,@PaymentId   
                                          ,@PaymentStatus,@Remark,@Marks,@MoreInfo,@CreatedBy,@CreatedDate,0)
                                          SELECT CAST(SCOPE_IDENTITY() as bigint)";
                var id = await dbConnection.QuerySingleAsync<int>(query, competition);
                result = id;
                if (result > 0)
                {

                    await AddCompetitionMember(id, competition.CompetitionMembers);
                    await AddCompettionImages(id, competition.CompettionImage);
                }

            }
            return result;
        }

        public async Task<List<CompetitionNameModel>> GetCompetitionName()
        {
            List<CompetitionNameModel> competitions = new List<CompetitionNameModel>();

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"select Id,CompetitionName from tblCompetitionType where IsDeleted=0";
                var comptition = await dbConnection.QueryAsync<CompetitionNameModel>(query);
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
                var query = @"select Id,ZPGATName,ClientId from tblZPGATMaster where IsDeleted=0 ";
                var zPGATs = await dbConnection.QueryAsync<ZPGATModel>(query);
                zPGATModels = zPGATs.ToList();
            }
            return zPGATModels;

        }
        public async Task<long> AddCompetitionMember(long id, List<CompetitionMember> CompetitionMembers)
        {

            int result = 0;
            using (DbConnection dbConnection = sqlreaderConnection)
            {

                {
                    foreach (CompetitionMember member in CompetitionMembers)
                    {

                        try
                        {
                            member.CompetitionId = id;
                            //skill.SkillId = id;

                            var query1 = @"Insert into tblCompetitionMembers(CompetitionId,DesignationId,PersonName,MobileNo,IsDeleted)
                                       values(@CompetitionId,@DesignationId,@PersonName,@MobileNo,0)";

                            var res = await dbConnection.ExecuteAsync(query1, member);
                        }
                        catch (Exception) { }
                    }
                }
                return result;
            }

        }
        public async Task<long> AddCompettionImages(long id, List<CompettionImages> compettionImages)
        {
            int result = 0;
            using (DbConnection dbConnection = sqlreaderConnection)
            {
                {
                    foreach (CompettionImages images in compettionImages)
                    {
                        try
                        {

                            images.CompetitionId = id;
                            var query1 = @"Insert into tblCompetitionImages(CompetitionId,ImagePath,IsMainImage,IsImage,IsDeleted)
                                       values(@CompetitionId,@ImagePath,@IsMainImage,@IsImage,0)";
                            var res = await dbConnection.ExecuteAsync(query1, images);
                        }
                        catch (Exception) { }
                    }
                }
                return result;
            }

        }


    }
}
