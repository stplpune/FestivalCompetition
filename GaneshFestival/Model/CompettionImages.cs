namespace GaneshFestival.Model
{
    public class CompettionImages : BaseModel
    {
        public long Id { get; set; }
        public long CompetitionId { get; set; }
        public string ImagePath { get; set; }
        public bool IsMainImage { get; set; }
        public long IsImage { get; set; }
        public override string Key => $"{this.Id}";

    }
}
