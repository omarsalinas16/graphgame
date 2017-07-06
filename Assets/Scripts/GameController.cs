using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FormBehaviour))]
public class GameController : MonoBehaviour {
	public static GameController Instance { get; private set; }

	[Header("Requirements")]
	[SerializeField]
	private Transform formSpawn;

	[Header("Points")]
	[SerializeField]
	private int maxTransformAttempts = 5;
	private int _transformAttempts;
	
	public int transformAttempts {
		get {
			return _transformAttempts;
		}

		set {
			_transformAttempts = value;

			if (UIController.Instance) {
				UIController.Instance.setTransformAttempsLabel(_transformAttempts);
			}
		}
	}

	[Header("Planes")]
	[SerializeField]
	public Transform xPlane;
	[SerializeField]
	public Transform zPlane;

	[SerializeField]
	private float axisPositionMin = -3.0f;
	[SerializeField]
	private float axisPositionMax = 3.0f;
	[SerializeField]
	private float axisStep = 0.3f;

	[Header("Shapes")]
	[SerializeField]
	private string formTag = "Form";
	[SerializeField]
	private Form[] forms;
	private Transform activeForm;

	private int shapeIndex = 0;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;
	}

	private void Start() {
		spawnForm();
		makeBothHoles();
		fadeInForm();
	}
	
	/*
	private void Update() {
		if ( Input.GetKey(KeyCode.DownArrow) ) {
			putRandomTransform();
			makeWholeHole(xPlane);
			makeHole(zPlane);
		}
	}
	
	private void putRandomTransform() {
		reposition();
		rescale();
		rotateRandom();
	}
	*/
	
	//beggin
	//Added by eduardo

	private void makeBothHoles() {
		makeHole(xPlane, Vector3.right);
		makeHole(zPlane, Vector3.forward);
	}
	
	private void makeHole(Transform plane, Vector3 moveAxis) {
		Vector3 originalPos = plane.position;
		float axisPosition = axisPositionMax;

		Mesh mesh = null;

		while (axisPosition > axisPositionMin) {
			plane.position = moveAxis * axisPosition;
			mesh = CSG.Subtract(plane.gameObject, activeForm.gameObject);

			MeshFilter meshFilter = plane.GetComponent<MeshFilter>();

			if (meshFilter != null && mesh != null) {
				meshFilter.sharedMesh = mesh;
				meshFilter.sharedMesh.RecalculateNormals();
				meshFilter.sharedMesh.RecalculateTangents();

				//change mesh collider too.
			}

			plane.position = originalPos;
			plane.localScale = Vector3.one;

			axisPosition -= axisStep;
		}
	}
	
	/*
	private void reposition() {
		GameObject form = activeForm.gameObject;
		newPosition = new Vector3(
								putRandomPosition(), 
								putRandomPosition(), 
								putRandomPosition());
		form.transform.position = newPosition;
	}
	
	private void rescale() {
		GameObject form = activeForm.gameObject;
		newScale = new Vector3(
								putRandomPosition(), 
								putRandomPosition(), 
								putRandomPosition());
		form.transform.localScale = newScale;
	}
	
	private void rotateRandom() {
		activeForm.Rotate(Vector3.up, putRandomPosition());
		activeForm.Rotate(Vector3.right, putRandomPosition());
		activeForm.Rotate(Vector3.forward, putRandomPosition());
	}
	
	private float putRandomPosition() {
		float randomN = Mathf.Round(Random.Range(-2.0f, 2.0f));

		if (randomN == 0) {
			randomN = 0.5f;
		}
		
		return Mathf.Abs(randomN);
	}
	*/
	//end

	public void initTransformAttempts() {
		transformAttempts = maxTransformAttempts;
	}

	private void spawnForm() {
		if (forms.Length > 0 && shapeIndex >= 0) {
			Form selectedForm = forms[shapeIndex];

			activeForm = Instantiate(selectedForm.shape, formSpawn.position, Quaternion.identity, formSpawn);

			if (PlayerController.Instance) {
				PlayerController.Instance.setActiveForm(getActiveForm());
			}

			initTransformAttempts();
		}
	}

	private void fadeInForm() {
		if (activeForm) {
			if (FormBehaviour.Instance) {
				FormBehaviour.Instance.fadeIn(activeForm);
			}
		}
	}
	
	private void endCurrentLevel() {
		Debug.Log("WIN");

		if (FormBehaviour.Instance) {
			FormBehaviour.Instance.fadeOut(activeForm);
		}

		if (shapeIndex < forms.Length - 1) {
			shapeIndex++;
			spawnForm();
		}
	}

	public Transform searchActiveForm() {
		GameObject form = GameObject.FindGameObjectWithTag(formTag);
		return form ? form.transform : null;
	}

	public Transform getActiveForm() {
		return activeForm ? activeForm : searchActiveForm();
	}
}
