using SQLite4Unity3d;

namespace Model {
    public class MovementInGame  {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        [Indexed]
        public int GamePlayedId {get; set;}        

        public int NumberTry {get; set;}
        public string Movement {get; set;}
        public int TypeTranform { get; set; } 

        override public string ToString() {
            return "Id " + Id + " GamePlayedId: " + GamePlayedId + " NumberTry: " + NumberTry + " Movement: " + Movement + " TypeTranform: " + TypeTranform;
        }       
    }

    public enum TypeTransform {
        POSITION, ROTATION, SCALE
    }
}
