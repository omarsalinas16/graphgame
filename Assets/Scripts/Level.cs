using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {

	public string name { get; set; }
	public Transform form { get; set; }
	
	public Vector3 startPosition { get; set; }
    public List<Rotation> startRotations { get; set; }
    public Vector3 startScale { get; set; }
	
	public Vector3 position { get; set; }
	public List<Rotation> rotations { get; set; }    
	public Vector3 scale { get; set; }
	
	public int maxSolveAttempts { get; set; }
	public int maxTransformations { get; set; }

    public class Rotation {
        public GameInputController.ROTATION_AXIS Axis { get; set; }
        public float Value { get; set; }
    }    
}
