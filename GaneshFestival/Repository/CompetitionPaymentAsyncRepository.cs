using Dapper;
using GaneshFestival.Model;
using GaneshFestival.Repository.Interface;
using System.Data.Common;
using System.Text;
using XSystem.Security.Cryptography;


namespace GaneshFestival.Repository
{
    public class CompetitionPaymentAsyncRepository : BaseAsyncRepository, ICompetitionPaymentAsyncRepository
    {
        int VehicleOwnerId = 0;
        IConfiguration configuration;

        private string hashSequence;
        private string Authorization_PayU;
        public string BkdLocationEditTime_Min;

     //  public DbConnection SqlReaderConnection { get; private set; }

        public CompetitionPaymentAsyncRepository(IConfiguration configuration) : base(configuration)
        {
            hashSequence = configuration.GetSection("globalvariables:hashSequence").Value;
            Authorization_PayU = configuration.GetSection("globalvariables:Authorization_PayU").Value;
            BkdLocationEditTime_Min = configuration.GetSection("globalvariables:BkdLocationEditTime_Min").Value;
        }
        public async Task<ReturnMsg> SaveUpdateVehiclePayment(VehiclePayment vehiclePayment)
        {
            ReturnMsg returnMsgs = new ReturnMsg();

            bool IsApproved = false;
            bool IsPaymentSuccess = false;
            long ApprovedBy = vehiclePayment.UserId;
            DateTime ApprovedDate = DateTime.Now;
            string ApprovedRemark = "Auto approved for online transfer (payment gateway).";
            int VehicleOwnerId = 0;
            string UserMobileno = null;
            string MobileNos = null;
            string VehicleNos = null;
            decimal Rate = 0;
            decimal BasicGSTAmount = 0;
            int result = 0;
            int result1 = 0;
            int result2 = 0;
            int VehiclePaymentTransactionId = 0;
            bool IsSuccess = false;
            string Msg = null;
            string PaymentStatus = null;
            IsApproved = true;
            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();

                VehicleOwnerId = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select top 1 Isnull(VehicleOwnerId,0) VehicleOwnerId from tbluser where id=@UserId  ", new { UserId = vehiclePayment.UserId });

                UserMobileno = await dbConnection.QueryFirstOrDefaultAsync<string>(@" select top 1 MobileNo1 from tbluser where id=@UserId  ", new { UserId = vehiclePayment.UserId });

                MobileNos = await dbConnection.QueryFirstOrDefaultAsync<string>(@" select @MobileNo +(case when @MobileNo!=(select top 1 MobileNo from tblvehicleowner where id=@VehicleOwnerId) 
                                                                                                then ','+(select top 1 MobileNo from tblvehicleowner where id=@VehicleOwnerId) else '' end)              
                                                                                              +(case when @MobileNo!=@UserMobileNo and 
                                                                                    (select top 1 MobileNo from tblvehicleowner where id=@VehicleOwnerId)!=@UserMobileNo
                                                                                        then ','+@UserMobileNo else '' end)             ",
                                                                                     new { MobileNo = vehiclePayment.MobileNo, VehicleOwnerId = VehicleOwnerId, UserMobileNo = UserMobileno });
                VehicleNos = await dbConnection.QueryFirstOrDefaultAsync<string>(@"select STUFF((SELECT ', ' + t1.VehicleRegistrationNo                            
                                                                                          FROM tblvehical t1           
                                                                                           WHERE t1.id in (select * from dbo.split(@VehicleIds,','))   FOR XML PATH (''))  , 1, 1, '')  ",
                                                                                     new { VehicleIds = vehiclePayment.VehicleIds });
                if (vehiclePayment.NoOfVehicles != 0)
                {
                    Rate = await dbConnection.QueryFirstOrDefaultAsync<decimal>(@"select top 1 Rate from vts_tblRateDetails where IsDeleted=0 and AmcTypeId=@AmcTypeId  ", new { AmcTypeId = vehiclePayment.AmcTypeId });

                    vehiclePayment.BasicAmount = Rate * vehiclePayment.NoOfVehicles;

                }
                else
                {
                    Rate = 0;
                    vehiclePayment.NoOfVehicles = 0;
                    vehiclePayment.BasicAmount = vehiclePayment.PaymentAmount - vehiclePayment.GSTAmount - vehiclePayment.TransactionCost;
                }
                BasicGSTAmount = vehiclePayment.BasicAmount + vehiclePayment.GSTAmount;

                if (vehiclePayment.Status == "success")
                {
                    IsPaymentSuccess = true;
                }
                else
                {
                    IsPaymentSuccess = false;
                }
                if (vehiclePayment.PayuMoneyId == "" || vehiclePayment.PayuMoneyId == "0")
                {
                    vehiclePayment.PayuMoneyId = null;
                }
                if (vehiclePayment.PayuMoneyId == null)
                {
                    result = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select 1 from vts_tblVehiclePaymentTransaction where isdeleted=0 and TransactionId=@TransactionId ", new { TransactionId = vehiclePayment.TransactionId });
                    if (result != 1)
                    {
                        VehiclePaymentTransactionId = await dbConnection.QueryFirstOrDefaultAsync<int>(@" insert into vts_tblVehiclePaymentTransaction                                                    
                                                                                 (VehicleOwnerId,GSTNo,NoOfVehicle,Rate,                                            
                                                                                 Amount,AdditionalCharges,IsPaymentSuccess,TransactionId,PayuMoneyId,Status,                                                  
                                                                                 Mode,Error,PgType,BankRefNum,                                                  
                                                                                 ProductInfo,FirstName,EmailId,MobileNo,CreatedBy,CreatedThr,                                
                                                                                 BasicAmount,GSTAmount,TransactionCost,BasicGSTAmount,                              
                                                                                 AutoManualType,PaymentDate,                      
                                                                                 IsPaymentByGateway,ModeOfPaymentId,            
                                                                                  [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark] )                                                    
                                                                                 values                                                    
                                                                                 (@VehicleOwnerId,@GSTNo,@NoOfVehicle,@Rate,                                            
                                                                                 @PaymentAmount,@AdditionalCharges,@IsPaymentSuccess,@TransactionId,@PayuMoneyId,@Status,                                                  
                                                                                 @Mode,@Error,@PgType,@BankRefNum,                                                  
                                                                                 @ProductInfo,@FirstName,@EmailId,@MobileNo,@UserId,@CreatedThr,                                
                                                                                 @BasicAmount,@GSTAmount,@TransactionCost,@BasicGSTAmount,                              
                                                                                 1,getdate(),                      
                                                                                 1,7,            
                                                                                 @IsApproved,@ApprovedBy,@ApprovedDate,@ApprovedRemark);SELECT CAST(SCOPE_IDENTITY() as bigint)  ",
                                                                                     new
                                                                                     {
                                                                                         VehicleOwnerId = VehicleOwnerId,
                                                                                         GSTNo = vehiclePayment.GSTNo,
                                                                                         NoOfVehicle = vehiclePayment.NoOfVehicles,
                                                                                         Rate = Rate,
                                                                                         PaymentAmount = vehiclePayment.PaymentAmount,
                                                                                         AdditionalCharges = vehiclePayment.AdditionalCharges,
                                                                                         IsPaymentSuccess = IsPaymentSuccess,
                                                                                         TransactionId = vehiclePayment.TransactionId,
                                                                                         PayuMoneyId = vehiclePayment.PayuMoneyId,
                                                                                         Status = vehiclePayment.Status,
                                                                                         Mode = vehiclePayment.Mode,
                                                                                         Error = vehiclePayment.Error,
                                                                                         PgType = vehiclePayment.PgType,
                                                                                         BankRefNum = vehiclePayment.BankRefNum,
                                                                                         ProductInfo = vehiclePayment.ProductInfo,
                                                                                         FirstName = vehiclePayment.FirstName,
                                                                                         EmailId = vehiclePayment.EmailId,
                                                                                         MobileNo = vehiclePayment.MobileNo,
                                                                                         UserId = vehiclePayment.UserId,
                                                                                         CreatedThr = vehiclePayment.CreatedThr,
                                                                                         BasicAmount = vehiclePayment.BasicAmount,
                                                                                         GSTAmount = vehiclePayment.GSTAmount,
                                                                                         TransactionCost = vehiclePayment.TransactionCost,
                                                                                         BasicGSTAmount = BasicGSTAmount,
                                                                                         IsApproved = IsApproved,
                                                                                         ApprovedBy = ApprovedBy,
                                                                                         ApprovedDate = ApprovedDate,
                                                                                         ApprovedRemark = ApprovedRemark
                                                                                     });
                    }
                    else
                    {
                        int id = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select Top 1 Id from vts_tblVehiclePaymentTransaction where isdeleted=0 and TransactionId=@TransactionId ", new { TransactionId = vehiclePayment.TransactionId });

                        VehiclePaymentTransactionId = await dbConnection.QueryFirstOrDefaultAsync<int>(@" update vts_tblVehiclePaymentTransaction                        
                                                                                                       set IsPaymentSuccess=@IsPaymentSuccess,                          
                                                                                                       Status=@Status,                                                          
                                                                                                       PaymentDate=getdate(),                        
                                                                                                       PayuMoneyId=@PayuMoneyId,                        
                                                                                                       Mode=@Mode,                        
                                                                                                       Error=@Error,                        
                                                                                                       PgType=@PgType,                        
                                                                                                       BankRefNum=@BankRefNum,                                                                  
                                                                                                       ProductInfo=@ProductInfo,              
                                                                                                       EmailId=@EmailId,              
                                                                                                       MobileNo=@MobileNo                   
                                                                                                       where id=@Id       ",
                                                                                     new
                                                                                     {

                                                                                         IsPaymentSuccess = IsPaymentSuccess,
                                                                                         PayuMoneyId = vehiclePayment.PayuMoneyId,
                                                                                         Status = vehiclePayment.Status,
                                                                                         Mode = vehiclePayment.Mode,
                                                                                         Error = vehiclePayment.Error,
                                                                                         PgType = vehiclePayment.PgType,
                                                                                         BankRefNum = vehiclePayment.BankRefNum,
                                                                                         ProductInfo = vehiclePayment.ProductInfo,
                                                                                         EmailId = vehiclePayment.EmailId,
                                                                                         MobileNo = vehiclePayment.MobileNo,
                                                                                         Id = id
                                                                                     });

                        VehiclePaymentTransactionId = id;
                    }
                    result2 = await dbConnection.QueryFirstOrDefaultAsync<int>(@" insert into vts_tblVehiclePaymentTransaction_History                              
                                                                                (Fk_Id,VehicleOwnerId,GSTNo,NoOfVehicle,Rate,Amount,BasicAmount,GSTAmount,
                                                                                TransactionCost,BasicGSTAmount,AdditionalCharges,IsPaymentSuccess,TransactionId                              
                                                                                ,PayuMoneyId,Status,Mode,Error,PgType,BankRefNum,ProductInfo,FirstName,EmailId,MobileNo
                                                                                ,CreatedThr,CreatedBy,IsDeleted,CreatedDate,AutoManualType,UpdatedBy,UpdatedDate,PaymentDate,            
                                                                                [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark])                              
                                                                                select Id,VehicleOwnerId,GSTNo,NoOfVehicle,Rate,Amount,BasicAmount,GSTAmount,
                                                                                TransactionCost,BasicGSTAmount,AdditionalCharges,IsPaymentSuccess,TransactionId                              
                                                                                ,PayuMoneyId,Status,Mode,Error,PgType,BankRefNum,ProductInfo,FirstName,EmailId,MobileNo,
                                                                                CreatedThr,CreatedBy,IsDeleted,CreatedDate,AutoManualType,@UserId,getdate(),PaymentDate,            
                                                                                [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark]                              
                                                                                 from vts_tblVehiclePaymentTransaction where Id=@Id        ",
                                                                                new
                                                                                {
                                                                                    Id = VehiclePaymentTransactionId,
                                                                                    UserId = vehiclePayment.UserId

                                                                                });

                    int tCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehiclePaymentTranId=@Id", new { Id = VehiclePaymentTransactionId });
                    int vCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from dbo.split(@VehicleIds,',')", new { VehicleIds = vehiclePayment.VehicleIds });

                    if (tCunt < vCunt)
                    {
                        result1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@"   insert into vts_tblvehiclepaymenttrandetails                        
                                                                                             (VehicleId,AmcTypeId,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,CreatedBy)                        
                                                                                                select items,2,@Id,@Rate,                        
                                                                                             (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then convert(date,getdate()) else dateadd(DAY,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),              
          
                                                                                                (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then dateadd(year,1,convert(date,getdate())) else dateadd(year,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),  @UserId from dbo.split(@VehicleIds,',')                        
                                                                                            ", new { VehicleIds = vehiclePayment.VehicleIds, UserId = vehiclePayment.UserId, Rate = Rate, Id = VehiclePaymentTransactionId });
                    }

                    if (IsPaymentSuccess == true)
                    {
                        //int tCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehiclePaymentTranId=@Id", new { Id = VehiclePaymentTransactionId });
                        //int vCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from dbo.split(@VehicleIds,',')", new { VehicleIds = vehiclePayment.VehicleIds });

                        //if (tCunt < vCunt)
                        //{
                        //    result1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@"   insert into vts_tblvehiclepaymenttrandetails                        
                        //                                                                     (VehicleId,AmcTypeId,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,CreatedBy)                        
                        //                                                                        select items,2,@Id,@Rate,                        
                        //                                                                     (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then convert(date,getdate()) else dateadd(DAY,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),              

                        //                                                                        (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then dateadd(year,1,convert(date,getdate())) else dateadd(year,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),  @UserId from dbo.split(@VehicleIds,',')                        
                        //                                                                    ", new { VehicleIds = vehiclePayment.VehicleIds, UserId = vehiclePayment.UserId, Rate = Rate, Id = VehiclePaymentTransactionId });
                        //}

                        int aCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from vts_tblVehicleAmc where isdeleted=0 and VehiclePaymentTranId=@Id", new { Id = VehiclePaymentTransactionId });
                        int mCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from dbo.split(@VehicleIds,',')", new { VehicleIds = vehiclePayment.VehicleIds });

                        if (aCunt < mCunt)
                        {
                            result1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@"                
                                                                                            DECLARE @OutputIds TABLE (ID bigint)                
                                                                                            DECLARE @OutputIds_device TABLE (ID bigint)                                                         
                                                                                            DECLARE @OutputIds_sim TABLE (ID bigint) 


                                                                                            insert into vts_tblVehicleAmc                
                                                                                            (VehicleOwnerId,VehicleId,DeviceCompanyId,DeviceId,MovementLocationId,VehicleAssignStatusId,IsGST,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,CreatedBy)                
                                                                                            OUTPUT inserted.Id into @OutputIds                                            
                                                                                            select @VehicleOwnerId,items,                  
                                                                                            (select top 1 DeviceCompanyId from vts_tblGPSDeviceStock where isdeleted=0 and IsAssignedToVehicleid=1 and VehicleId=items),                  
                                                                                            (select top 1 DeviceId from vts_tblGPSDeviceStock where isdeleted=0 and IsAssignedToVehicleid=1 and VehicleId=items),                  
                                                                                            (select top 1 MovementLocationId from vts_tblGPSDeviceStock where isdeleted=0 and IsAssignedToVehicleid=1 and VehicleId=items),                  
                                                                                            7,1,@Id,(@PaymentAmount/@NoOfVehicle),                  
                                                                                            (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then convert(date,getdate()) else dateadd(DAY,1,max(AmcToDate)) end) from vts_tblVehicleAmc where isdeleted=0 and VehicleId=items),                              
              
                                                                                            (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then dateadd(year,1,convert(date,getdate())) else dateadd(year,1,max(AmcToDate)) end) from vts_tblVehicleAmc where isdeleted=0 and VehicleId=items),             
                               
                                                                                            @UserId                  
                                                                                            from dbo.split(@VehicleIds,',') 


                                                                                        insert into vts_tblvehicleamc_history                
                                                                                            ([Fk_Id],VehicleId,MovementLocationId,VehicleAssignStatusId,IsGST,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,                
                                                                                            CreatedBy,[CreatedDate],[IsDeleted],                
                                                                                            [UpdatedBy],[UpdatedDate])                
                                                                                            select [Id] ,[VehicleId],[MovementLocationId],[VehicleAssignStatusId],[IsGST] ,[VehiclePaymentTranId] ,[Rate],[AmcFromDate],[AmcToDate],                
                                                                                              [CreatedBy],[CreatedDate],[IsDeleted],                
                                                                                              @UserId,getdate()                
                                                                                            from vts_tblVehicleAmc                 
                                                                                            where id in (select ID from @OutputIds)                
               
                                                                                            --change VehicleAssignStatusId=7 renew        
                                                                                              --or max(AmcToDate)<convert(date,getdate())  new condn for renew at 17-07-2020 11am      
                                                                                             update dbo.vts_tblGPSDeviceStock                                                
                                                                                             set           
                                                                                             [VehicleAssignStatusId]=7,                                                
                                                                                             [VehicleAssignedDate]=getdate(),                                               
                                                                                             VehicleAssignedByUserId=@UserId,              
                                                                                             UpdatedBy=@UserId,              
                                                                                             UpdatedDate=getdate()         
                                                                                             OUTPUT inserted.Id into @OutputIds_device                                                 
                                                                                             where IsDeleted=0 and IsAssignedtoVehicleId=1        
                                                                                             and VehicleId in (select items from dbo.split(@VehicleIds,',') )                        
                                                  
                                                                                             insert into dbo.vts_tblGPSDeviceVehicleAssign                                                
                                                                                             ([DeviceCompanyId]      ,[DeviceId]  , MovementLocationId    , [IsAssignedToVehicleid],VehicleId,ReferenceDeviceCompanyId,ReferenceDeviceId,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                       
  
                                                                                             ,[CreatedBy]      ,[CreatedDate]      ,[IsDeleted])                                            
                                                                                             select [DeviceCompanyId]      ,[DeviceId], MovementLocationId      , [IsAssignedToVehicleid],VehicleId,ReferenceDeviceCompanyId,ReferenceDeviceId,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                 
                                                                                                ,@UserId      ,getdate()      ,[IsDeleted]                                            
                                                                                             from dbo.vts_tblGPSDeviceStock                                          
                                                                                             where Id in (select ID from @OutputIds_device)                                              
                                           
                                                                                             update dbo.vts_tblGPSDeviceSimNoStock                      
                                                                                             set                                               
                                                                                             [VehicleAssignStatusId]=7,                                    
                                                                                             [VehicleAssignedDate]=getdate(),                                               
                                                                                             VehicleAssignedByUserId=@UserId         
                                                                                             OUTPUT inserted.Id into @OutputIds_sim                                               
                                                                                             where IsDeleted=0 and IsAssignedtoVehicleId=1        
                                                                                             and VehicleId in (select items from dbo.split(@VehicleIds,',') )                                                        
                                        
                                                                                             insert into vts_tblSimVehicleAssign                                                
                                                                                             ([SimCompanyId]      ,[SImId],[DeviceSimNo] , MovementLocationId , [IsAssignedToVehicleid],[VehicleId],ReferenceDeviceSimNo,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                                
                                                                                             ,[CreatedBy]      ,[CreatedDate]      ,[IsDeleted])                                                 
                                                                                             select [SimCompanyId]      ,[SimId] ,DeviceSimNo , MovementLocationId   , [IsAssignedToVehicleid],[VehicleId],ReferenceDeviceSimNo,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                           
                                                                                             ,@UserId      ,getdate()      ,[IsDeleted]                                               
                                                                                             from dbo.vts_tblGPSDeviceSimNoStock                                                
                                                                                             where Id in (select ID from @OutputIds_sim)   

                                                                                            ", new { VehicleOwnerId = VehicleOwnerId, PaymentAmount = vehiclePayment.PaymentAmount, NoOfVehicle = vehiclePayment.NoOfVehicles, VehicleIds = vehiclePayment.VehicleIds, UserId = vehiclePayment.UserId, Id = VehiclePaymentTransactionId });


                        }
                        IsSuccess = true;
                        Msg = "Payment Successfully Done.";
                        PaymentStatus = "Payment Successfully Done. GPS device (Vehicle No. " + @VehicleNos + ") service has been renewed for one year.";


                    }
                    else
                    {
                        IsSuccess = false;
                        Msg = "Payment Failed. Please try again.";
                        PaymentStatus = "We are sorry that your payment for (Vehicle No. " + @VehicleNos + ") is failed. Pls try again. If the amount has been deducted, please call to call center 020-24261344.";

                    }

                }
                else
                {
                    result = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select 1 from vts_tblVehiclePaymentTransaction where isdeleted=0 and PayuMoneyId=@PayuMoneyId ", new { PayuMoneyId = vehiclePayment.PayuMoneyId });
                    if (result != 1)
                    {
                        result = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select 1 from vts_tblVehiclePaymentTransaction where isdeleted=0 and TransactionId=@TransactionId ", new { TransactionId = vehiclePayment.TransactionId });
                        if (result != 1)
                        {
                            VehiclePaymentTransactionId = await dbConnection.QueryFirstOrDefaultAsync<int>(@" insert into vts_tblVehiclePaymentTransaction                                                    
                                                                                 (VehicleOwnerId,GSTNo,NoOfVehicle,Rate,                                            
                                                                                 Amount,AdditionalCharges,IsPaymentSuccess,TransactionId,PayuMoneyId,Status,                                                  
                                                                                 Mode,Error,PgType,BankRefNum,                                                  
                                                                                 ProductInfo,FirstName,EmailId,MobileNo,CreatedBy,CreatedThr,                                
                                                                                 BasicAmount,GSTAmount,TransactionCost,BasicGSTAmount,                              
                                                                                 AutoManualType,PaymentDate,                      
                                                                                 IsPaymentByGateway,ModeOfPaymentId,            
                                                                                  [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark] )                                                    
                                                                                 values                                                    
                                                                                 (@VehicleOwnerId,@GSTNo,@NoOfVehicle,@Rate,                                            
                                                                                 @PaymentAmount,@AdditionalCharges,@IsPaymentSuccess,@TransactionId,@PayuMoneyId,@Status,                                                  
                                                                                 @Mode,@Error,@PgType,@BankRefNum,                                                  
                                                                                 @ProductInfo,@FirstName,@EmailId,@MobileNo,@UserId,@CreatedThr,                                
                                                                                 @BasicAmount,@GSTAmount,@TransactionCost,@BasicGSTAmount,                              
                                                                                 1,getdate(),                      
                                                                                 1,7,            
                                                                                 @IsApproved,@ApprovedBy,@ApprovedDate,@ApprovedRemark);SELECT CAST(SCOPE_IDENTITY() as bigint)  ",
                                                                                         new
                                                                                         {
                                                                                             VehicleOwnerId = VehicleOwnerId,
                                                                                             GSTNo = vehiclePayment.GSTNo,
                                                                                             NoOfVehicle = vehiclePayment.NoOfVehicles,
                                                                                             Rate = Rate,
                                                                                             PaymentAmount = vehiclePayment.PaymentAmount,
                                                                                             AdditionalCharges = vehiclePayment.AdditionalCharges,
                                                                                             IsPaymentSuccess = IsPaymentSuccess,
                                                                                             TransactionId = vehiclePayment.TransactionId,
                                                                                             PayuMoneyId = vehiclePayment.PayuMoneyId,
                                                                                             Status = vehiclePayment.Status,
                                                                                             Mode = vehiclePayment.Mode,
                                                                                             Error = vehiclePayment.Error,
                                                                                             PgType = vehiclePayment.PgType,
                                                                                             BankRefNum = vehiclePayment.BankRefNum,
                                                                                             ProductInfo = vehiclePayment.ProductInfo,
                                                                                             FirstName = vehiclePayment.FirstName,
                                                                                             EmailId = vehiclePayment.EmailId,
                                                                                             MobileNo = vehiclePayment.MobileNo,
                                                                                             UserId = vehiclePayment.UserId,
                                                                                             CreatedThr = vehiclePayment.CreatedThr,
                                                                                             BasicAmount = vehiclePayment.BasicAmount,
                                                                                             GSTAmount = vehiclePayment.GSTAmount,
                                                                                             TransactionCost = vehiclePayment.TransactionCost,
                                                                                             BasicGSTAmount = BasicGSTAmount,
                                                                                             IsApproved = IsApproved,
                                                                                             ApprovedBy = ApprovedBy,
                                                                                             ApprovedDate = ApprovedDate,
                                                                                             ApprovedRemark = ApprovedRemark
                                                                                         });
                        }
                        else
                        {
                            int id = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select Top 1 Id from vts_tblVehiclePaymentTransaction where isdeleted=0 and TransactionId=@TransactionId ", new { TransactionId = vehiclePayment.TransactionId });

                            VehiclePaymentTransactionId = await dbConnection.QueryFirstOrDefaultAsync<int>(@" update vts_tblVehiclePaymentTransaction                        
                                                                                                       set IsPaymentSuccess=@IsPaymentSuccess,                          
                                                                                                       Status=@Status,                                                          
                                                                                                       PaymentDate=getdate(),                        
                                                                                                       PayuMoneyId=@PayuMoneyId,                        
                                                                                                       Mode=@Mode,                        
                                                                                                       Error=@Error,                        
                                                                                                       PgType=@PgType,                        
                                                                                                       BankRefNum=@BankRefNum,                                                                  
                                                                                                       ProductInfo=@ProductInfo,              
                                                                                                       EmailId=@EmailId,              
                                                                                                       MobileNo=@MobileNo                   
                                                                                                       where id=@Id       ",
                                                                                         new
                                                                                         {

                                                                                             IsPaymentSuccess = IsPaymentSuccess,
                                                                                             PayuMoneyId = vehiclePayment.PayuMoneyId,
                                                                                             Status = vehiclePayment.Status,
                                                                                             Mode = vehiclePayment.Mode,
                                                                                             Error = vehiclePayment.Error,
                                                                                             PgType = vehiclePayment.PgType,
                                                                                             BankRefNum = vehiclePayment.BankRefNum,
                                                                                             ProductInfo = vehiclePayment.ProductInfo,
                                                                                             EmailId = vehiclePayment.EmailId,
                                                                                             MobileNo = vehiclePayment.MobileNo,
                                                                                             Id = id
                                                                                         });

                            VehiclePaymentTransactionId = id;
                        }

                        result2 = await dbConnection.QueryFirstOrDefaultAsync<int>(@" insert into vts_tblVehiclePaymentTransaction_History                              
                                                                                (Fk_Id,VehicleOwnerId,GSTNo,NoOfVehicle,Rate,Amount,BasicAmount,GSTAmount,
                                                                                TransactionCost,BasicGSTAmount,AdditionalCharges,IsPaymentSuccess,TransactionId                              
                                                                                ,PayuMoneyId,Status,Mode,Error,PgType,BankRefNum,ProductInfo,FirstName,EmailId,MobileNo
                                                                                ,CreatedThr,CreatedBy,IsDeleted,CreatedDate,AutoManualType,UpdatedBy,UpdatedDate,PaymentDate,            
                                                                                [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark])                              
                                                                                select Id,VehicleOwnerId,GSTNo,NoOfVehicle,Rate,Amount,BasicAmount,GSTAmount,
                                                                                TransactionCost,BasicGSTAmount,AdditionalCharges,IsPaymentSuccess,TransactionId                              
                                                                                ,PayuMoneyId,Status,Mode,Error,PgType,BankRefNum,ProductInfo,FirstName,EmailId,MobileNo,
                                                                                CreatedThr,CreatedBy,IsDeleted,CreatedDate,AutoManualType,@UserId,getdate(),PaymentDate,            
                                                                                [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark]                              
                                                                                 from vts_tblVehiclePaymentTransaction where Id=@Id        ",
                                                                                new
                                                                                {
                                                                                    Id = VehiclePaymentTransactionId,
                                                                                    UserId = vehiclePayment.UserId

                                                                                });
                        int tCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehiclePaymentTranId=@Id", new { Id = VehiclePaymentTransactionId });
                        int vCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from dbo.split(@VehicleIds,',')", new { VehicleIds = vehiclePayment.VehicleIds });

                        if (tCunt < vCunt)
                        {
                            result1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@"   insert into vts_tblvehiclepaymenttrandetails                        
                                                                                             (VehicleId,AmcTypeId,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,CreatedBy)                        
                                                                                                select items,2,@Id,@Rate,                        
                                                                                             (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then convert(date,getdate()) else dateadd(DAY,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),              
          
                                                                                                (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then dateadd(year,1,convert(date,getdate())) else dateadd(year,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),  @UserId from dbo.split(@VehicleIds,',')                        
                                                                                            ", new { VehicleIds = vehiclePayment.VehicleIds, UserId = vehiclePayment.UserId, Rate = Rate, Id = VehiclePaymentTransactionId });
                        }

                        if (IsPaymentSuccess == true)
                        {
                            //int tCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehiclePaymentTranId=@Id", new { Id = VehiclePaymentTransactionId });
                            //int vCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from dbo.split(@VehicleIds,',')", new { VehicleIds = vehiclePayment.VehicleIds });

                            //if (tCunt < vCunt)
                            //{
                            //    result1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@"   insert into vts_tblvehiclepaymenttrandetails                        
                            //                                                                 (VehicleId,AmcTypeId,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,CreatedBy)                        
                            //                                                                    select items,2,@Id,@Rate,                        
                            //                                                                 (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then convert(date,getdate()) else dateadd(DAY,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),              

                            //                                                                    (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then dateadd(year,1,convert(date,getdate())) else dateadd(year,1,max(AmcToDate)) end) from vts_tblvehiclepaymenttrandetails where isdeleted=0 and VehicleId=items),  @UserId from dbo.split(@VehicleIds,',')                        
                            //                                                                ", new { VehicleIds = vehiclePayment.VehicleIds, UserId = vehiclePayment.UserId, Rate = Rate, Id = VehiclePaymentTransactionId });
                            //}

                            int aCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from vts_tblVehicleAmc where isdeleted=0 and VehiclePaymentTranId=@Id", new { Id = VehiclePaymentTransactionId });
                            int mCunt = await dbConnection.QueryFirstOrDefaultAsync<int>(@"select count(1) from dbo.split(@VehicleIds,',')", new { VehicleIds = vehiclePayment.VehicleIds });

                            if (aCunt < mCunt)
                            {
                                result1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@"                
                                                                                            DECLARE @OutputIds TABLE (ID bigint)                
                                                                                            DECLARE @OutputIds_device TABLE (ID bigint)                                                         
                                                                                            DECLARE @OutputIds_sim TABLE (ID bigint) 


                                                                                            insert into vts_tblVehicleAmc                
                                                                                            (VehicleOwnerId,VehicleId,DeviceCompanyId,DeviceId,MovementLocationId,VehicleAssignStatusId,IsGST,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,CreatedBy)                
                                                                                            OUTPUT inserted.Id into @OutputIds                                            
                                                                                            select @VehicleOwnerId,items,                  
                                                                                            (select top 1 DeviceCompanyId from vts_tblGPSDeviceStock where isdeleted=0 and IsAssignedToVehicleid=1 and VehicleId=items),                  
                                                                                            (select top 1 DeviceId from vts_tblGPSDeviceStock where isdeleted=0 and IsAssignedToVehicleid=1 and VehicleId=items),                  
                                                                                            (select top 1 MovementLocationId from vts_tblGPSDeviceStock where isdeleted=0 and IsAssignedToVehicleid=1 and VehicleId=items),                  
                                                                                            7,1,@Id,(@PaymentAmount/@NoOfVehicle),                  
                                                                                            (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then convert(date,getdate()) else dateadd(DAY,1,max(AmcToDate)) end) from vts_tblVehicleAmc where isdeleted=0 and VehicleId=items),                              
              
                                                                                            (select top 1 (case when max(AmcToDate) is null or max(AmcToDate)<convert(date,getdate()) then dateadd(year,1,convert(date,getdate())) else dateadd(year,1,max(AmcToDate)) end) from vts_tblVehicleAmc where isdeleted=0 and VehicleId=items),             
                               
                                                                                            @UserId                  
                                                                                            from dbo.split(@VehicleIds,',') 


                                                                                        insert into vts_tblvehicleamc_history                
                                                                                            ([Fk_Id],VehicleId,MovementLocationId,VehicleAssignStatusId,IsGST,VehiclePaymentTranId,Rate,AmcFromDate,AmcToDate,                
                                                                                            CreatedBy,[CreatedDate],[IsDeleted],                
                                                                                            [UpdatedBy],[UpdatedDate])                
                                                                                            select [Id] ,[VehicleId],[MovementLocationId],[VehicleAssignStatusId],[IsGST] ,[VehiclePaymentTranId] ,[Rate],[AmcFromDate],[AmcToDate],                
                                                                                              [CreatedBy],[CreatedDate],[IsDeleted],                
                                                                                              @UserId,getdate()                
                                                                                            from vts_tblVehicleAmc                 
                                                                                            where id in (select ID from @OutputIds)                
               
                                                                                            --change VehicleAssignStatusId=7 renew        
                                                                                              --or max(AmcToDate)<convert(date,getdate())  new condn for renew at 17-07-2020 11am      
                                                                                             update dbo.vts_tblGPSDeviceStock                                                
                                                                                             set           
                                                                                             [VehicleAssignStatusId]=7,                                                
                                                                                             [VehicleAssignedDate]=getdate(),                                               
                                                                                             VehicleAssignedByUserId=@UserId,              
                                                                                             UpdatedBy=@UserId,              
                                                                                             UpdatedDate=getdate()         
                                                                                             OUTPUT inserted.Id into @OutputIds_device                                                 
                                                                                             where IsDeleted=0 and IsAssignedtoVehicleId=1        
                                                                                             and VehicleId in (select items from dbo.split(@VehicleIds,',') )                        
                                                  
                                                                                             insert into dbo.vts_tblGPSDeviceVehicleAssign                                                
                                                                                             ([DeviceCompanyId]      ,[DeviceId]  , MovementLocationId    , [IsAssignedToVehicleid],VehicleId,ReferenceDeviceCompanyId,ReferenceDeviceId,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                       
  
                                                                                             ,[CreatedBy]      ,[CreatedDate]      ,[IsDeleted])                                            
                                                                                             select [DeviceCompanyId]      ,[DeviceId], MovementLocationId      , [IsAssignedToVehicleid],VehicleId,ReferenceDeviceCompanyId,ReferenceDeviceId,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                 
                                                                                                ,@UserId      ,getdate()      ,[IsDeleted]                                            
                                                                                             from dbo.vts_tblGPSDeviceStock                                          
                                                                                             where Id in (select ID from @OutputIds_device)                                              
                                           
                                                                                             update dbo.vts_tblGPSDeviceSimNoStock                      
                                                                                             set                                               
                                                                                             [VehicleAssignStatusId]=7,                                    
                                                                                             [VehicleAssignedDate]=getdate(),                                               
                                                                                             VehicleAssignedByUserId=@UserId         
                                                                                             OUTPUT inserted.Id into @OutputIds_sim                                               
                                                                                             where IsDeleted=0 and IsAssignedtoVehicleId=1        
                                                                                             and VehicleId in (select items from dbo.split(@VehicleIds,',') )                                                        
                                        
                                                                                             insert into vts_tblSimVehicleAssign                                                
                                                                                             ([SimCompanyId]      ,[SImId],[DeviceSimNo] , MovementLocationId , [IsAssignedToVehicleid],[VehicleId],ReferenceDeviceSimNo,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                                
                                                                                             ,[CreatedBy]      ,[CreatedDate]      ,[IsDeleted])                                                 
                                                                                             select [SimCompanyId]      ,[SimId] ,DeviceSimNo , MovementLocationId   , [IsAssignedToVehicleid],[VehicleId],ReferenceDeviceSimNo,[VehicleAssignStatusId],[VehicleAssignedDate],[VehicleAssignedByUserId]                                           
                                                                                             ,@UserId      ,getdate()      ,[IsDeleted]                                               
                                                                                             from dbo.vts_tblGPSDeviceSimNoStock                                                
                                                                                             where Id in (select ID from @OutputIds_sim)   

                                                                                            ", new { VehicleOwnerId = VehicleOwnerId, PaymentAmount = vehiclePayment.PaymentAmount, NoOfVehicle = vehiclePayment.NoOfVehicles, VehicleIds = vehiclePayment.VehicleIds, UserId = vehiclePayment.UserId, Id = VehiclePaymentTransactionId });


                            }
                            IsSuccess = true;
                            Msg = "Payment Successfully Done.";
                            PaymentStatus = "Payment Successfully Done. GPS device (Vehicle No. " + @VehicleNos + ") service has been renewed for one year.";


                        }
                        else
                        {
                            IsSuccess = false;
                            Msg = "Payment Failed. Please try again.";
                            PaymentStatus = "We are sorry that your payment for (Vehicle No. " + @VehicleNos + ") is failed. Pls try again. If the amount has been deducted, please call to call center 020-24261344.";

                        }

                    }
                    else
                    {
                        int id1 = await dbConnection.QueryFirstOrDefaultAsync<int>(@" select top 1 Id from vts_tblVehiclePaymentTransaction where isdeleted=0 and PayuMoneyId=@PayuMoneyId ", new { PayuMoneyId = vehiclePayment.PayuMoneyId });
                        bool IsPaymentSuccess_Old = await dbConnection.QueryFirstOrDefaultAsync<bool>(@" select top 1 IsPaymentSuccess from tblPlotPaymentTransaction where isdeleted=0 and PayuMoneyId=@PayuMoneyId", new { PayuMoneyId = vehiclePayment.PayuMoneyId });

                        if (IsPaymentSuccess_Old == IsPaymentSuccess)
                        {
                            IsSuccess = false;
                            if (IsPaymentSuccess == true)
                            {
                                Msg = "PaymentId (" + vehiclePayment.PayuMoneyId + ") already exists with same payment status Success.";
                            }
                            else
                            {
                                Msg = "PaymentId (" + vehiclePayment.PayuMoneyId + ") already exists with same payment status Failure.";
                            }
                        }
                        else
                        {
                            if (IsPaymentSuccess == true)
                            {
                                VehiclePaymentTransactionId = await dbConnection.QueryFirstOrDefaultAsync<int>(@" update vts_tblVehiclePaymentTransaction                        
                                                                                                       set IsPaymentSuccess=@IsPaymentSuccess,                          
                                                                                                       Status=@Status,                                                          
                                                                                                       PaymentDate=getdate(),                        
                                                                                                       PayuMoneyId=@PayuMoneyId,                        
                                                                                                       Mode=@Mode,                        
                                                                                                       Error=@Error,                        
                                                                                                       PgType=@PgType,                        
                                                                                                       BankRefNum=@BankRefNum,                                                                  
                                                                                                       ProductInfo=@ProductInfo,              
                                                                                                       EmailId=@EmailId,              
                                                                                                       MobileNo=@MobileNo                   
                                                                                                       where id=@Id       ",
                                                                                           new
                                                                                           {

                                                                                               IsPaymentSuccess = IsPaymentSuccess,
                                                                                               PayuMoneyId = vehiclePayment.PayuMoneyId,
                                                                                               Status = vehiclePayment.Status,
                                                                                               Mode = vehiclePayment.Mode,
                                                                                               Error = vehiclePayment.Error,
                                                                                               PgType = vehiclePayment.PgType,
                                                                                               BankRefNum = vehiclePayment.BankRefNum,
                                                                                               ProductInfo = vehiclePayment.ProductInfo,
                                                                                               EmailId = vehiclePayment.EmailId,
                                                                                               MobileNo = vehiclePayment.MobileNo,
                                                                                               Id = id1
                                                                                           });
                                VehiclePaymentTransactionId = id1;

                                result2 = await dbConnection.QueryFirstOrDefaultAsync<int>(@" insert into vts_tblVehiclePaymentTransaction_History                              
                                                                                (Fk_Id,VehicleOwnerId,GSTNo,NoOfVehicle,Rate,Amount,BasicAmount,GSTAmount,
                                                                                TransactionCost,BasicGSTAmount,AdditionalCharges,IsPaymentSuccess,TransactionId                              
                                                                                ,PayuMoneyId,Status,Mode,Error,PgType,BankRefNum,ProductInfo,FirstName,EmailId,MobileNo
                                                                                ,CreatedThr,CreatedBy,IsDeleted,CreatedDate,AutoManualType,UpdatedBy,UpdatedDate,PaymentDate,            
                                                                                [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark])                              
                                                                                select Id,VehicleOwnerId,GSTNo,NoOfVehicle,Rate,Amount,BasicAmount,GSTAmount,
                                                                                TransactionCost,BasicGSTAmount,AdditionalCharges,IsPaymentSuccess,TransactionId                              
                                                                                ,PayuMoneyId,Status,Mode,Error,PgType,BankRefNum,ProductInfo,FirstName,EmailId,MobileNo,
                                                                                CreatedThr,CreatedBy,IsDeleted,CreatedDate,AutoManualType,@UserId,getdate(),PaymentDate,            
                                                                                [IsApproved],[ApprovedBy],[ApprovedDate],[ApprovedRemark]                              
                                                                                 from vts_tblVehiclePaymentTransaction where Id=@Id        ",
                                                                               new
                                                                               {
                                                                                   Id = VehiclePaymentTransactionId,
                                                                                   UserId = vehiclePayment.UserId

                                                                               });

                            }
                            else
                            {
                                IsSuccess = false;
                                Msg = "Payment Failed. Please try again.";
                                PaymentStatus = "We are sorry that your payment for (PaymentId " + vehiclePayment.PayuMoneyId + ") is failed. Pls try again. If the amount has been deducted, please call to call center 020-24261344.";
                            }
                        }
                    }
                }

                returnMsgs.IsSuccess = IsSuccess;
                returnMsgs.Msg = Msg;
                returnMsgs.MobileNos = MobileNos;
                returnMsgs.PaymentStatus = PaymentStatus;


            }
            return returnMsgs;
        }

        public async Task<List<PayUCredentials>> getPayUCredentials()
        {
            List<PayUCredentials> payUCredentials = null;

            using (DbConnection dbConnection = sqlreaderConnection)
            {
                await dbConnection.OpenAsync();
                var sqlQuery = string.Format(@"       declare @key varchar(200)                                      
                                                      declare @salt varchar(200)                                      
                                                      declare @BaseUrl varchar(200)                                      
                                                      declare @responseUrl varchar(200)                                      
                                        
                                                      set @key=(select top 1 [Key] from tblPayuCredentials where isdeleted=0) --'A20mZWAj'                                      
                                                      set @salt=(select top 1 Salt from tblPayuCredentials where isdeleted=0) --'DiZb35uZOq'                                      
                                                      set @BaseUrl=(select top 1 BaseUrl from tblPayuCredentials where isdeleted=0) --'https://test.payu.in' --https://secure.payu.in                                      
                                                      set @responseUrl=  (select top 1 ResponseUrl_VTS from tblPayuCredentials where isdeleted=0) --'http://mahamining.com/ResponseHandlingVTS.aspx' --'http://mahamining.com/ResponseHandling.aspx'                                    
  
                                                      select                                   
                                                        @key as [Key],                                      
                                                        @salt as Salt,                                      
                                                        @BaseUrl as BaseUrl,                                      
                                                        @responseUrl as ResponseUrl     
                                                    ");

                var payu = await dbConnection.QueryAsync<PayUCredentials>(sqlQuery);

                payUCredentials = payu.ToList();
            }
            return payUCredentials;
        }
        public async Task<string> Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        public async Task<hashandTranid> PaymentGateway_Hash(HashModel hashModels)
        {

            hashandTranid returnMsg = new hashandTranid();
            var payuCredentials = await getPayUCredentials();
            List<PayUCredentials> payUCredentials = null;
            payUCredentials = payuCredentials.ToList();
            string key = payUCredentials[0].Key;
            string surl = "", furl = "", curl = "";
            string[] hashVarsSeq;
            string hash_string = "";

            string action1 = "";
            string hash = "";
            string txnid1 = "";
            // generating txnid

            Random rnd = new Random();
            string strHash = await Generatehash512(rnd.ToString() + DateTime.Now);
            txnid1 = strHash.ToString().Substring(0, 20);
            if (
           string.IsNullOrEmpty(key) ||
           string.IsNullOrEmpty(txnid1) ||
           string.IsNullOrEmpty(hashModels.amount) ||
           string.IsNullOrEmpty(hashModels.firstname) ||
           string.IsNullOrEmpty(hashModels.email) ||
           string.IsNullOrEmpty(hashModels.phone) ||
           string.IsNullOrEmpty(hashModels.productinfo) ||
           //string.IsNullOrEmpty(surl) ||
           //string.IsNullOrEmpty(furl) ||
           string.IsNullOrEmpty(hashModels.service_provider)
           )
            {
                //error


            }
            else
            {
                hashVarsSeq = hashSequence.Split('|'); // spliting hash sequence from config
                hash_string = "";
                foreach (string hash_var in hashVarsSeq)
                {
                    if (hash_var == "key")
                    {
                        hash_string = hash_string + key.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "txnid")
                    {
                        hash_string = hash_string + txnid1.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "amount")
                    {
                        hash_string = hash_string + Convert.ToDecimal(hashModels.amount).ToString("g29").Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "firstname")
                    {
                        hash_string = hash_string + hashModels.firstname.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "email")
                    {
                        hash_string = hash_string + hashModels.email.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "phone")
                    {
                        hash_string = hash_string + hashModels.phone.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "productinfo")
                    {
                        hash_string = hash_string + hashModels.productinfo.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "surl")
                    {
                        hash_string = hash_string + surl.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "furl")
                    {
                        hash_string = hash_string + furl.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "service_provider")
                    {
                        hash_string = hash_string + hashModels.service_provider.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf1") //Flag
                    {
                        hash_string = hash_string + hashModels.udf1.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf2") //UserId
                    {
                        hash_string = hash_string + hashModels.udf2.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf3") //VehicleIds
                    {
                        hash_string = hash_string + hashModels.udf3.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf4") // GSTNo
                    {
                        hash_string = hash_string + hashModels.udf4.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf5") //NoOfVehicles$BasicAmt$GSTAmt$TransactionCost 
                    {
                        hash_string = hash_string + hashModels.udf5.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf6")
                    {
                        hash_string = hash_string + hashModels.udf6.Trim();
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf7")
                    {
                        hash_string = hash_string + hashModels.udf7.Trim();
                        hash_string = hash_string + '|';
                    }
                    else
                    {

                        hash_string = hash_string + "";// isset if else
                        hash_string = hash_string + '|';
                    }
                }

                hash_string += payUCredentials[0].Salt;// appending SALT

                hash = await Generatehash512(hash_string);         //generating hash
                //action1 = PAYU_BASE_URL + "/_payment";// setting URL

                //string msg = await Cheeckhash(hash, hash_string);
                returnMsg.hash = hash;
                returnMsg.TransactionId = txnid1;

            }
            return returnMsg;
        }
        public async Task<long> UpdatePaymentStatus(PaymentStatusModel payment)
        {
            var query = "";
            long result = 0;
            using (DbConnection dbConnection = sqlreaderConnection)
            {
                query = @"Update tblCompetition set PaymentId =@PaymentId , PaymentStatus=@PaymentStatus where Id=@CompetitionId ";
                result = await dbConnection.ExecuteAsync(query, new { CompetitionId = payment.CompetitionId, PaymentId = payment.PaymentId, PaymentStatus = payment.PaymentStatus });

                    query = @"Insert into tblPaymentResponse(PaymentId,payuMoneyId,amount,status,ResponseStr,isdeleted) VALUES (
                                           @PaymentId,@payuMoneyId,@amount,@PaymentStatus,@ResponseStr,0)";
                    result = await dbConnection.ExecuteAsync(query, new {  PaymentId = payment.PaymentId, PaymentStatus = payment.PaymentStatus, payuMoneyId = payment.payuMoneyId, amount = payment.amount, ResponseStr=payment.ResponseStr });
                
            }
            return result;
        }

  
        

        
    }
}
