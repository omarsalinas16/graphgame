using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {
	public string name = "Level";
	public Transform form = null;

	[Header("Start state")]
	public Vector3 startPosition = new Vector3(0, 0, 0);
	public Vector3 startRotation = new Vector3(0, 0, 0);
	public Vector3 startScale = new Vector3(1, 1, 1);

	[Header("Solution state")]
	public Vector3 position = new Vector3(0, 0, 0);
	public Vector3 rotation = new Vector3(0, 0, 0);
	public Vector3 scale = new Vector3(1, 1, 1);

	[Header("Level properties")]
	public int maxSolveAttempts = 1;
	public int maxTransformations = 1;
}
