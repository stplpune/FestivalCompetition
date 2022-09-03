using XAct;

namespace GaneshFestival.Model
{
    public class CompetitionNameModel
    {
        public long Id { get; set; }
        public string CompetitionName { get; set; }

    }
    public class CompetitionModel : BaseModel
    {
        public long Id { get; set; }
        public long CompetitionTypeId { get; set; }
        public long ZPGATId { get; set; }
        public long ClientId { get; set; }
        public string VillageName { get; set; }
        public string PersonName { get; set; }

        public decimal Amount { get; set; }
        public string PaymentScreenPath { get; set; }
        public string VideoPath { get; set; }
        public string PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public string Remark { get; set; }
        public decimal Marks { get; set; }
        public string MoreInfo { get; set; }
        public string MobileNo { get; set; }
        public List<CompetitionMember> CompetitionMembers { get; set; }
        public List<CompettionImages> CompettionImage { get; set; }
        public override string Key => $"{this.Id}";
    }

    public class PaymentStatusModel
    {
        public long CompetitionId { get; set; }
        public string PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public string payuMoneyId { get; set; }
        public decimal amount { get; set; }
        public string ResponseStr { get; set; }
    }

    public class clsCompetitionData
    {
        public long Id { get; set; }
        public long CompetitionTypeId { get; set; }
        public string CompetitionType { get; set; }

        public long ZPGATId { get; set; }
        public string ZPGAT { get; set; }

        public long ClientId { get; set; }
        public string VillageName { get; set; }
        public string PersonName { get; set; }

        public decimal Amount { get; set; }
        public string PaymentScreenPath { get; set; }
        public string VideoPath { get; set; }
        public string PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public string Remark { get; set; }
        public decimal Marks { get; set; }
        public string MoreInfo { get; set; }
        public string MobileNo { get; set; }
    }

    public class clsMemberData
    {
        public string DesignationName { get; set; }
        public int DesignationId { get; set; }
        public string PersonName { get; set; }
        public string MobileNo { get; set; }
    }

    public class ClsImageData
    {
        public string ImagePath { get; set; }
    }

    public class clsCompetitionOtherData
    {
        public List<clsMemberData> MemberList { get; set; }
        public List<ClsImageData> ImageList { get; set; }

    }


    public class clsDashboardData
    {
        public int SrNo { get; set; }
        public string ZPGATName { get; set; }
        public int MandalCount { get; set; }
        public int PersonalCount { get; set; }
    }

}