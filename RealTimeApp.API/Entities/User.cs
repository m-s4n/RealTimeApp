namespace RealTimeApp.API.Entities
{
    public class User:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Team Team { get; set; }
        public int TeamId { get; set; }
    }
}
