using UnityEngine;

[System.Serializable]
public class Level {

	public string name { get; set; }
	public Transform form { get; set; }
	
	public Vector3 startPosition { get; set; }
	public Vector3 startRotation { get; set; }
	public Vector3 startScale { get; set; }
	
	public Vector3 position { get; set; }
	public Vector3 rotation { get; set; }
	public Vector3 scale { get; set; }
	
	public int maxSolveAttempts { get; set; }
	public int maxTransformations { get; set; }
}
