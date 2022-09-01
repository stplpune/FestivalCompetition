namespace GaneshFestival.Model
{
    public class CompetitionNameModel 
    {
        public long Id { get; set; }
        public string CompetitionName { get; set; }

    }
    public class CompetitionModel:BaseModel
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
        public List<CompetitionMember> CompetitionMembers { get; set; }
        public List<CompettionImages> CompettionImage { get; set; }
        public override string Key => $"{this.Id}";
    }

}