﻿using System.Collections;
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
	[SerializeField]
	private GameObject referenceCubes;

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
	private Transform xPlane;
	[SerializeField]
	private Transform zPlane;
	[SerializeField]
	private GameObject xPlaneGO;
	[SerializeField]
	private GameObject zPlaneGO;

	[SerializeField]
	private float axisPositionMin = -3.0f;
	[SerializeField]
	private float axisPositionMax = 3.0f;
	[SerializeField]
	private float axisStep = 0.3f;

	private PlaneBehaviour xPlaneBehaviour;
	private PlaneBehaviour zPlaneBehaviour;

	private bool movePlanes = false;

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

		if (xPlane) {
			xPlaneBehaviour = xPlane.GetComponent<PlaneBehaviour>();
		}

		if (zPlane) {
			zPlaneBehaviour = zPlane.GetComponent<PlaneBehaviour>();
		}
	}
	
	private void Update() {
		if (Input.GetKeyDown(KeyCode.DownArrow) ) {
			movePlanes = true;
		}

		if (movePlanes) {
			if (!xPlaneBehaviour.allowMovement) {
				xPlaneBehaviour.allowMovement = true;
			}

			if (xPlaneBehaviour.hasEnded && !xPlaneBehaviour.hasCollided) {
				if (!zPlaneBehaviour.allowMovement) {
					zPlaneBehaviour.allowMovement = true;
				}
			}

			if (zPlaneBehaviour.hasEnded) {
				movePlanes = false;
			}
		}
	}
	
	private void appearOrRotate(int verAngle, int angle) {
		if (!referenceCubes) return;

		if (referenceCubes.transform.eulerAngles.y == verAngle) {
				referenceCubes.transform.Rotate(Vector3.up * angle);
				referenceCubes.SetActive(true);
		} else {
			if (referenceCubes.activeSelf) {
				referenceCubes.SetActive(false);
			} else  {
				referenceCubes.SetActive(true);
			}
		}
	}

	private void makeBothHoles() {
		makeHole(xPlaneGO, Vector3.right);
		makeHole(zPlaneGO, Vector3.forward);
	}
	
	private void makeHole(GameObject plane, Vector3 moveAxis) {
		Vector3 originalPos = plane.transform.position;
		float axisPosition = axisPositionMax;

		Mesh mesh = null;

		while (axisPosition > axisPositionMin) {
			plane.transform.position = moveAxis * axisPosition;
			mesh = CSG.Subtract(plane.gameObject, activeForm.gameObject);

			if (mesh != null) {
				mesh.name = "Generated Mesh";
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();

				MeshFilter meshFilter = plane.GetComponent<MeshFilter>();

				if (meshFilter != null) {
					//meshFilter.sharedMesh = null;
					DestroyImmediate(plane.GetComponent<MeshFilter>());
					MeshFilter filter = plane.AddComponent<MeshFilter>();
					//meshFilter.sharedMesh = mesh;
					filter.sharedMesh = mesh;
				}

				MeshCollider planeCollider = plane.GetComponent<MeshCollider>();

				if (planeCollider != null) {
					//planeCollider.sharedMesh = null;
					DestroyImmediate(plane.GetComponent<MeshCollider>());
					MeshCollider collider = plane.AddComponent<MeshCollider>();
					//planeCollider.sharedMesh = mesh;
					collider.sharedMesh = mesh;
				}
			}

			plane.transform.position = originalPos;
			plane.transform.localScale = Vector3.one;

			axisPosition -= axisStep;
		}
	}

	public void initTransformAttempts() {
		transformAttempts = maxTransformAttempts;
	}

	private void spawnForm() {
		instantiateForm();
		setFormToSolution();
		makeBothHoles();
		fadeInForm();
		setFormToIdentity();
	}

	private void instantiateForm() {
		if (forms.Length > 0 && shapeIndex >= 0) {
			Form selectedForm = forms[shapeIndex];

			activeForm = Instantiate(selectedForm.shape, formSpawn.position, Quaternion.identity, formSpawn);

			if (PlayerController.Instance) {
				PlayerController.Instance.setActiveForm(getActiveForm());
			}

			initTransformAttempts();
		}
	}

	private void setFormToSolution() {
		if (activeForm && forms[shapeIndex] != null) {
			activeForm.localPosition = forms[shapeIndex].position;
			activeForm.Rotate(forms[shapeIndex].rotation);
			activeForm.localScale = forms[shapeIndex].scale;
		}
	}

	private void setFormToIdentity() {
		if (activeForm) {
			activeForm.localPosition = Vector3.zero;
			activeForm.rotation = Quaternion.identity;
			activeForm.localScale = Vector3.one;
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
			instantiateForm();
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
