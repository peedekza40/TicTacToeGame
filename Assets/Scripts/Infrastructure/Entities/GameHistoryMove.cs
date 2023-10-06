using SQLite.Attributes;
namespace Infrastructure.Entities
{
    [UnityEngine.Scripting.Preserve]
    public class GameHistoryMove
    {
        [PrimaryKey] 
        [AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public int GameHistoryID { get; set; }

        public string Player { get; set; }
        public int Turn { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

}