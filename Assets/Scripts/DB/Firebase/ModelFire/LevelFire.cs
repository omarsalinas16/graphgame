using SimpleFirebaseUnity.MiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DB.Firebase.ModelFire
{
    public class LevelFire
    {

        private const string TRANSLATION = "translation";
        private const string ROTATION = "rotation";
        private const string SCALE = "scale";

        public string Name { get; set; }
        public string StartState { get; set; }
        public string SolvedState { get; set; }
        public string PrefabName { get; set; }
        public int MaxSolveAttemps { get; set; }
        public int MaxTransformations { get; set; }

        public LevelFire(string levelJSON, string name)
        {
            this.Name = name;
            var level = Json.Deserialize(levelJSON) as Dictionary<string, object>;            
            MaxSolveAttemps = (int)((Int64)level["max_attemps"]);
            MaxTransformations = (int)((Int64)level["max_transformations"]);            
            StartState = Json.Serialize(level["start_state"]);
            SolvedState = Json.Serialize(level["solved_state"]);
        }

        public Level ToLevel(LevelController formLevel)
        {
            Dictionary<string, Vector3> startState = ConvertState2Transform(StartState);
            Dictionary<string, Vector3> solvedState = ConvertState2Transform(SolvedState);            

            return new Level
            {
                name = this.Name,
                startPosition = startState[TRANSLATION],
                startRotation = startState[ROTATION],
                startScale = startState[SCALE],
                position = solvedState[TRANSLATION],
                rotation = solvedState[ROTATION],
                scale = solvedState[SCALE],
                maxSolveAttempts = this.MaxSolveAttemps,
                maxTransformations = this.MaxTransformations,
                form = formLevel.GetTranformWithPrefabName(this.PrefabName)
            };
        }

        public Dictionary<string,Vector3> ConvertState2Transform(string stateJSON) {
            var state = Json.Deserialize(stateJSON) as Dictionary<string, object>;
            var translation = (string)state[TRANSLATION];
            var rotation = (string)state[ROTATION];
            var scale = (string)state[SCALE];

            Dictionary<string, Vector3> transformation = new Dictionary<string, Vector3>();
            transformation.Add(TRANSLATION, GetVector3(translation));
            transformation.Add(ROTATION, GetVector3(rotation));
            transformation.Add(SCALE, GetVector3(scale));

            return transformation;
        }

        /// <summary>
        /// This will return a vector3
        /// You have to pass a string in the next format [x],[y],[z] e.g. 0,0,0
        /// </summary>
        /// <param name="commaString"></param>
        /// <returns></returns>
        public Vector3 GetVector3(string commaString) {
            var valuesString = commaString.Split(',');
            var values = new int[valuesString.Length];
            for (int i = 0; i < valuesString.Length; i++) {
                values[i] = Int32.Parse(valuesString[i]);
            }

            Vector3 v = new Vector3(values[0], values[1], values[2]);
            Debug.Log(v);
            return v;
        }
    }
}
