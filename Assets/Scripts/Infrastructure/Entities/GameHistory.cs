using SQLite.Attributes;
namespace Infrastructure.Entities
{
    [UnityEngine.Scripting.Preserve]
    public class GameHistory
    {
        [PrimaryKey] 
        [AutoIncrement]
        public int ID { get; set; }

        public string Mode { get; set; }
        
        [NotNull]
        public int Size { get; set; }

        public string GameResult { get; set; }

        public string DateTime { get; set; }
    }

}