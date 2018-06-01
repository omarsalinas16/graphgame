using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

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
            string typeMovement = getStringTypeMovement(type);
            movements.Add(new Movement
            {
                sequence = sequence,
                typeMovement = typeMovement,
                movement = movement
            });
        }

        private string getStringTypeMovement(TYPE_MOVEMENT type) {
            string typeMovement = "";
            switch (type)
            {
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
            return typeMovement;
        }

        public string ToJson() {

            var movementsDict = new Dictionary<int, Dictionary<string, string>>();
            for (int i = 0; i < movements.Count(); i ++) {
                var m = movements[i];
                
                var moveDict = new Dictionary<string, string> {
                    { m.typeMovement, m.movement }
                };
                movementsDict.Add(i, moveDict);                
            }
            var gamePlayed = new Dictionary<string, object>
            {
                { "level", this.LevelId },
                { "user", this.UserUID },
                { "movements", movementsDict },
                { "solved", Solved}
            };
            var jsonGamePlayed = Json.Serialize(gamePlayed);

            return jsonGamePlayed;

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
            var jsonGamePlayed = gameFire.ToJson();
            Debug.Log(jsonGamePlayed);
            var dbFire = new DbFire();
            dbFire.InsertGamePlayed(
                delegate (SimpleFirebaseUnity.Firebase f, DataSnapshot d)
                {
                    Debug.Log("Push success");
                },
                delegate (SimpleFirebaseUnity.Firebase f, FirebaseError e)
                {
                    Debug.Log("Push error");
                },
                jsonGamePlayed
            );
            // TODO: if the json is ok then push into firebase
        }
    }

    public enum TYPE_MOVEMENT {
        ROTATE, SCALE, TRANSLATE
    }
}
