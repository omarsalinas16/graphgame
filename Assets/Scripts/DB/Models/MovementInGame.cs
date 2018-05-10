using SQLite4Unity3d;

namespace Model {
    public class MovementInGame  {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        [Indexed]
        public int GamePlayedId {get; set;}

        public int NumberTry {get; set;}
        public string Movement {get; set;}        
    }
}
