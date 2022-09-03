using Dapper;
using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
using System.Data.Common;
using System.Diagnostics.Metrics;

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
                var query = @"select Id,ZPGATName,ClientId,IsCity from tblZPGATMaster where IsDeleted=0 ";
                var zPGATs = await dbConnection.QueryAsync<ZPGATModel>(query);
                zPGATModels = zPGATs.ToList();
            }
            return zPGATModels;

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
                            var res = await dbConnection.ExecuteAsync(query1);
                        }
                        catch (Exception) { }
                    }
                }
                return result;
            }

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

        public async Task<List<clsCompetitionData>> GetCompetitionData(int CompetitionTypeId, int ZPGATId,
            string? SearhchText, int ClientId, int PageNo)
        {
            List<clsCompetitionData> competitions = new List<clsCompetitionData>();
            SearhchText = SearhchText == null ? "" : SearhchText;
            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"SELECT c.Id as Id,c.CompetitionTypeId as CompetitionTypeId,c.ZPGATId as ZPGATId,
               c.ClientId as ClientId,c.VillageName as VillageName,c.PersonName as PersonName,c.Amount as Amount,
               c.PaymentScreenPath as PaymentScreenPath,
                c.VideoPath as VideoPath,c.PaymentId as PaymentId,c.PaymentStatus as PaymentStatus,
               c.Remark as Remark,c.Marks as Marks,c.MoreInfo as MoreInfo,
                convert(varchar(20),c.CreatedDate,105) as CreatedDate,c.MobileNo as MobileNo,
                t.CompetitionName as CompetitionType,g.ZPGATName as ZPGAT
                FROM tblCompetition c 
                inner join tblPaymentResponse r on (c.Id=r.PaymentId)
                inner join tblCompetitionType t on (c.CompetitionTypeId=t.Id)
                inner join tblZPGATMaster g on (c.ZPGATId=g.Id) where c.isdeleted=0 and r.status='success'
               and (@CompetitionTypeId=0 or c.CompetitionTypeId=@CompetitionTypeId)
                and (@ZPGATId=0 or c.ZPGATId=@ZPGATId)
                and c.ClientId=@ClientId
               and (@SearhchText='' or c.PersonName like concat('%',@SearhchText,'%')
                  or c.MobileNo like concat('%',@SearhchText,'%'))";

                var comptition = await dbConnection.QueryAsync<clsCompetitionData>(query,
                    new
                    {
                        CompetitionTypeId = CompetitionTypeId,
                        ZPGATId = ZPGATId,
                        SearhchText = SearhchText,
                        ClientId = ClientId
                    });
                competitions = comptition.ToList();
            }
            return competitions;
        }



        public async Task<clsCompetitionOtherData> GetOtherCompetitionData(long CompetitionId)
        {
            clsCompetitionOtherData obj_othercompetitionData = new clsCompetitionOtherData();

            List<clsMemberData> memberlist = new List<clsMemberData>();
            List<ClsImageData> imagelist = new List<ClsImageData>();

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();

                // image
                var query = @"SELECT ImagePath FROM tblCompetitionImages where CompetitionId=@CompetitionId and isdeleted=0";

                var imglist = await dbConnection.QueryAsync<ClsImageData>(query,
                    new
                    {
                        CompetitionId = CompetitionId
                    });
                imagelist = imglist.ToList();



                // member
                //  var querym = @"SELECT distinct (case when DesignationId=1 then N'अध्यक्ष' 
                // when DesignationId = 2 then N'उपाध्यक्ष' else N'सदस्य' end) as DesignationName,DesignationId
                //          ,PersonName ,MobileNo FROM tblCompetitionMembers where CompetitionId=@CompetitionId and isdeleted=0";



                var querym = @"SELECT top 1(case when DesignationId = 1 then N'अध्यक्ष'
                        when DesignationId = 2 then N'उपाध्यक्ष' else N'सदस्य' end) as DesignationName,DesignationId
                        ,PersonName ,MobileNo FROM tblCompetitionMembers
                        where CompetitionId = @CompetitionId and DesignationId = 1 and isdeleted = 0
                        union all
                        SELECT top 1(case when DesignationId = 1 then N'अध्यक्ष'
                        when DesignationId = 2 then N'उपाध्यक्ष' else N'सदस्य' end) as DesignationName,DesignationId
                        ,PersonName ,MobileNo FROM tblCompetitionMembers
                        where CompetitionId = @CompetitionId and DesignationId = 2 and isdeleted = 0
                        union all
                        SELECT(case when DesignationId = 1 then N'अध्यक्ष'
                        when DesignationId = 2 then N'उपाध्यक्ष' else N'सदस्य' end) as DesignationName,DesignationId
                        ,PersonName ,MobileNo FROM tblCompetitionMembers
                        where CompetitionId = @CompetitionId and DesignationId = 3 and isdeleted = 0";


                var memberlst = await dbConnection.QueryAsync<clsMemberData>(querym,
                    new
                    {
                        CompetitionId = CompetitionId
                    });
                memberlist = memberlst.ToList();


                obj_othercompetitionData.ImageList = imagelist;
                obj_othercompetitionData.MemberList = memberlist;


            }
            return obj_othercompetitionData;
        }

        public async Task<List<clsDashboardData>> GetDashboardCount(int ClientId)
        {
            List<clsDashboardData> obj_dashboardcount = new List<clsDashboardData>();
            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"select ROW_NUMBER() over(order by tblZPGATMaster.id) as SrNo,tblZPGATMaster.ZPGATName,
                        (select count(id) from tblCompetition where tblCompetition.ZPGATId=tblZPGATMaster.Id and 
                        tblCompetition.IsDeleted=0 and tblCompetition.PaymentStatus='success' and CompetitionTypeId=1) as MandalCount,
                        (select count(id) from tblCompetition where tblCompetition.ZPGATId=tblZPGATMaster.Id and 
                        tblCompetition.IsDeleted=0 and tblCompetition.PaymentStatus='success' and CompetitionTypeId=2) as PersonalCount
                        from tblZPGATMaster 
                        where ClientId=@ClientId";

                var comptition = await dbConnection.QueryAsync<clsDashboardData>(query,
                    new
                    {
                        ClientId = ClientId
                    });
                obj_dashboardcount = comptition.ToList();
            }
            return obj_dashboardcount;
        }


        public async Task<long> UpdateMarks(long CompetitionId, decimal Marks, string Remark)
        {
            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var query = @"update tblCompetition set Marks=@Marks ,Remark=@Remark
                        where Id=@CompetitionId";

                var res = await dbConnection.ExecuteAsync(query,
                    new
                    {
                        CompetitionId = CompetitionId,
                        Marks = Marks,
                        Remark = Remark
                    });


            }
            return 1;
        }


    }
}
