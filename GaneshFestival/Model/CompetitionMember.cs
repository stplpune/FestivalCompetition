namespace GaneshFestival.Model
{
    public class CompetitionMember:BaseModel
    {
        public long Id { get; set; }
        public long CompetitionId { get; set; }
        public long DesignationId { get; set; }
        public long PersonName { get; set; }
        public string MobileNo { get; set; }

        public override string Key => $"{this.Id}";
    }
}
