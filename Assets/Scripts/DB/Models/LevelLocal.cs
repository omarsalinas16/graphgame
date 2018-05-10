using SQLite4Unity3d;

namespace Model {
    public class LevelLocal  {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartState { get; set; }
        public string SolvedState { get; set; }	
        public string PrefabName { get; set; } 
        public int MaxSolveAttemps { get; set; }
        public int MaxTransformations { get; set; }

        override public string ToString() {
            return Name + " " + StartState + " " + SolvedState + " " + PrefabName + " " + MaxSolveAttemps + " " + MaxTransformations;
        }

        public Level ToLevel(LevelController formLevel) {
            TransformState startState = TransformState.GetObjectFromJSON(this.StartState);
            TransformState solvedState = TransformState.GetObjectFromJSON(this.SolvedState);

            return new Level {
                name = this.Name,
                startPosition = startState.position,
                startRotation = startState.rotation,
                startScale = startState.scale,
                position = solvedState.position,
                rotation = solvedState.rotation,
                scale = solvedState.scale,
                maxSolveAttempts = this.MaxSolveAttemps,
                maxTransformations = this.MaxTransformations,
                form = formLevel.GetTranformWithPrefabName(this.PrefabName)
            };
        }
        
    }
}
