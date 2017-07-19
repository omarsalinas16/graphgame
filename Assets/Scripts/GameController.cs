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
	private float scaleOffsetFix = 0.05f;
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
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
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

	private void makeBothHoles() {
		makeHole(xPlane, Vector3.right);
		makeHole(zPlane, Vector3.forward);
	}

	private void makeHole(Transform plane, Vector3 moveAxis) {
		GameObject planeGO = plane.gameObject;
		Vector3 originalPos = plane.position;
		float axisPosition = axisPositionMax;

		Mesh mesh = null;

		while (axisPosition > axisPositionMin) {
			plane.position = moveAxis * axisPosition;
			mesh = CSG.Subtract(planeGO, activeForm.gameObject);

			if (mesh != null) {
				mesh.name = "Generated Mesh";
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();

				MeshFilter meshFilter = planeGO.GetComponent<MeshFilter>();

				if (meshFilter != null) {
					DestroyImmediate(planeGO.GetComponent<MeshFilter>());
					MeshFilter filter = planeGO.AddComponent<MeshFilter>();

					filter.sharedMesh = mesh;
				}

				MeshCollider planeCollider = planeGO.GetComponent<MeshCollider>();

				if (planeCollider != null) {
					DestroyImmediate(planeGO.GetComponent<MeshCollider>());
					MeshCollider collider = planeGO.AddComponent<MeshCollider>();

					collider.sharedMesh = mesh;
				}
			}

			plane.position = originalPos;
			plane.localScale = Vector3.one;

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

			// Temporal scale fix for plane colliders.
			activeForm.localScale = forms[shapeIndex].scale + new Vector3(scaleOffsetFix, scaleOffsetFix, scaleOffsetFix);
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
