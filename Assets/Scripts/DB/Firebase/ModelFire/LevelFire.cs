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

        private const string TRANSLATION = "position";
        private const string ROTATION = "rotation";
        private const string SCALE = "scale";

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public string StartState { get; set; }
        public string SolvedState { get; set; }
        public string PrefabName { get; set; }
        public float StepScale { get; set; }
        public float StepTranslate { get; set; }
        public int MaxSolveAttemps { get; set; }
        public int MaxTransformations { get; set; }

        public LevelFire(string levelJSON, string id)
        {            
            this.Id = id;
            var level = Json.Deserialize(levelJSON) as Dictionary<string, object>;
            var stepsScale = level["steps_scale"];
            var stepsTranslate = level["steps_translate"];
            if (stepsScale.GetType() == typeof(Int64)) {
                this.StepScale = (Int64)stepsScale;
            } else if (stepsScale.GetType() == typeof(Double))
            {
                this.StepScale = (float)((Double)stepsScale);
            }
            if (stepsTranslate.GetType() == typeof(Int64))
            {
                this.StepTranslate = (Int64)stepsTranslate;
            }
            else if (stepsTranslate.GetType() == typeof(Double))
            {
                this.StepTranslate = (float)((Double)stepsTranslate);
            }            
            this.Name = (string)level["name"];
            //this.StepScale = (float)((Int64)level["steps_scale"]);            
            //this.StepTranslate = (float)((Int64)level["steps_translate"]);
            //Debug.Log("Steps: " + StepScale + " " + StepTranslate);
            //this.Disabled = ((string)level["disabled"]) == "true" ? true : false;
            this.Disabled = (bool)level["disabled"];
            MaxSolveAttemps = (int)((Int64)level["max_attemps"]);
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
                startRotations = getListOfRotations(StartState),
                startScale = startState[SCALE],
                position = solvedState[TRANSLATION],
                rotations = getListOfRotations(SolvedState),
                scale = solvedState[SCALE],
                maxSolveAttempts = this.MaxSolveAttemps,
                maxTransformations = this.MaxTransformations,
                form = formLevel.GetTranformWithPrefabName(this.PrefabName),
                stepScale = this.StepScale,
                stepTranslate = this.StepTranslate
            };
        }

        private List<Level.Rotation> getListOfRotations(string stateJSON) {
            var state = Json.Deserialize(stateJSON) as Dictionary<string, object>;
            var rotations = state[ROTATION] as List<object>;
            /*var rotationsJson = Json.Serialize(state[ROTATION]);
            Debug.Log("Rotations: " + state[ROTATION]);
            var rotations = Json.Deserialize(rotationsJson) as Dictionary<string, object>;*/
            // TODO: check if this works
            //rotations.OrderBy(r => r.Key);

            List<Level.Rotation> rotationsList = new List<Level.Rotation>();
            foreach (object rs in rotations) {
                Debug.Log("Rot: " + rs);
                var rotationJson = Json.Serialize(rs);
                Debug.Log("Rot: " + rotationJson);
                var rotation = Json.Deserialize(rotationJson) as Dictionary<string, object>;
                foreach (KeyValuePair<string, object> r in rotation)
                {
                    GameInputController.ROTATION_AXIS axis = GameInputController.ROTATION_AXIS.X;
                    if (r.Key == "x")
                    {
                        axis = GameInputController.ROTATION_AXIS.X;

                    }
                    else if (r.Key == "y") {
                        axis = GameInputController.ROTATION_AXIS.Y;
                    }
                    else if (r.Key == "z")
                    {
                        axis = GameInputController.ROTATION_AXIS.Z;
                    }
                    Debug.Log("Axis: " + axis + " Value: " + r.Value);
                    rotationsList.Add(new Level.Rotation
                    {
                        Axis = axis,
                        Value = ((float)(Int64)r.Value)
                    });
                }                
            }
            return rotationsList;
        }

        public Dictionary<string,Vector3> ConvertState2Transform(string stateJSON) {
            var state = Json.Deserialize(stateJSON) as Dictionary<string, object>;
            var translation = (string)state[TRANSLATION];
            //var rotation = (string)state[ROTATION];
            var scale = (string)state[SCALE];

            Dictionary<string, Vector3> transformation = new Dictionary<string, Vector3>();
            transformation.Add(TRANSLATION, GetVector3(translation));
            //transformation.Add(ROTATION, GetVector3(rotation));
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

        public static string GetCommaVector3(Vector3 vector) {
            return vector.x + "," + vector.y + "," + vector.z;

        }
    }
}
