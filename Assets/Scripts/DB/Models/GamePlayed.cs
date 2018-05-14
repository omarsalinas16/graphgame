using SQLite4Unity3d;

namespace Model {
    public class GamePlayed  {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        [Indexed]
        public int LevelId { get; set; }         
        public int UserId { get; set; }         
        public bool Solved { get; set; }         

        override public string ToString() {
            return "Id: " + Id + " UserId: " + UserId + " LevelId: " + LevelId + " Solved: " + Solved;
        }
    }
}