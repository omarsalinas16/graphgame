using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DB.Firebase.ModelFire
{
    class GameFire
    {
        private string ROTATE = "rotate";
        private string SCALE = "scale";
        private string TRANSLATE = "translate";

        public string UserUID { get; set; }
        public string LevelId { get; set; }
        public List<Movement> movements { get; set; }
        public bool Solved { get; set; }

        public GameFire(string userId, string levelId) {
            UserUID = userId;
            LevelId = levelId;
            movements = new List<Movement>();
        }

        public class Movement {
            public int sequence { get; set; }
            public string typeMovement { get; set; }
            public string movement { get; set; }
        }

        public void AddMovement(TYPE_MOVEMENT type, string movement, int sequence) {
            string typeMovement = "";
            switch (type) {
                case TYPE_MOVEMENT.ROTATE:
                    typeMovement = ROTATE;
                    break;
                case TYPE_MOVEMENT.SCALE:
                    typeMovement = SCALE;
                    break;
                case TYPE_MOVEMENT.TRANSLATE:
                    typeMovement = TRANSLATE;
                    break;                
            }
            movements.Add(new Movement
            {
                sequence = sequence,
                typeMovement = typeMovement,
                movement = movement
            });
        }
    }

    class GameFireMaker {
        private GameFire gameFire;
        private int count;

        public GameFireMaker(string userUID, string levelId) {
            gameFire = new GameFire(userUID, levelId);
            count = 0;
        }

        public void AddMovement(TYPE_MOVEMENT type, Vector3 movement) {
            gameFire.AddMovement(type, LevelFire.GetCommaVector3(movement), count++);
        }

        public void End(bool solved) {
            gameFire.Solved = solved;
            // TODO: convert gameFire to JSON and insert in firebase
        }
    }

    public enum TYPE_MOVEMENT {
        ROTATE, SCALE, TRANSLATE
    }
}
