using SQLite4Unity3d;

namespace Model {
    public class GamePlayed  {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        [Indexed]
        public int LevelId {get; set;}           
        public bool Solved {get; set;}
    }
}