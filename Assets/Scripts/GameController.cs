using UnityEngine;
using Parabox.CSG;
using System.Collections.Generic;
using System;

public enum PlaneSequenceStatus {
	Idle = 0,
	MovingX = 1,
	MovingZ = 2,
	Ended = 3,
	Collided = 4
}

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FormBehaviour))]
public class GameController : MonoBehaviour {
	public static GameController Instance { get; private set; }

	public delegate void OnTryAttemptsChanged(int attempts);
	public event OnTryAttemptsChanged tryAttemptsChangedEvent;

	public delegate void OnTransformAttemptsChanged(int attempts);
	public event OnTransformAttemptsChanged transformAttemptsChangedEvent;

	private FormBehaviour _formBehaviour = null;
	private FormBehaviour formBehaviour {
		get {
			if (_formBehaviour == null) {
				if (FormBehaviour.Instance != null) {
					_formBehaviour = FormBehaviour.Instance;
				} else {
					_formBehaviour = FindObjectOfType<FormBehaviour>();
				}
			}

			return _formBehaviour;
		}
	}

	[Header("Requirements")]
	[SerializeField]
	private Transform formSpawn;

	[Header("Points")]
	[SerializeField]
	private int maxSolveTryAttempts = 3;
	[SerializeField]
	private int _solveTryAttempts;
	public int solveTryAttempts {
		get {
			return _solveTryAttempts;
		}

		set {
			_solveTryAttempts = Mathf.Clamp(value, 0, maxSolveTryAttempts);

			if (tryAttemptsChangedEvent != null) {
				tryAttemptsChangedEvent(_solveTryAttempts);
			}
		}
	}

	[SerializeField]
	private int maxTransformAttempts = 5;
	[SerializeField]
	private int _transformAttempts;
	public int transformAttempts {
		get {
			return _transformAttempts;
		}

		set {
			_transformAttempts = Mathf.Clamp(value, 0, maxTransformAttempts);

			if (transformAttemptsChangedEvent != null) {
				transformAttemptsChangedEvent(_transformAttempts);
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
	private bool issueMovePlane = false;
	public PlaneSequenceStatus planeSequenceStatus = PlaneSequenceStatus.Idle;

	[Header("Level")]
	public Level currentLevel = null;

	[Header("Forms")]
	[SerializeField]
	private string formTag = "Form";
	[SerializeField]
	private float scaleOffsetFix = 0.05f;
	private Transform activeForm;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Start() {
		// Delegates and event suscriptions

		UIController.Instance.resetGameEvent += resetTransformAttempts;

		if (xPlane) {
			xPlaneBehaviour = xPlane.GetComponent<PlaneBehaviour>();
		}

		if (zPlane) {
			zPlaneBehaviour = zPlane.GetComponent<PlaneBehaviour>();
		}

		setAndInitCurrentLevel();
	}

	private void Update() {
		if (Input.GetKey("l")) {
			modifyMeshCube();
		}
		if (Input.GetKey("p")) {
			scanFigure();
		}

		handlePlaneSequence();
	}

	private void makeBothHoles() {
		makeHole(xPlane, Vector3.right);
		makeHole(zPlane, Vector3.forward);
	}

	private void modifyMeshCube() {
		MeshFilter meshFilter = activeForm.gameObject.GetComponent<MeshFilter>();
		var collider = activeForm.gameObject.GetComponent<MeshCollider>();
		var boxCollider = activeForm.gameObject.GetComponent<BoxCollider>();
		boxCollider.enabled = false;

		if (!collider) {
			collider = activeForm.gameObject.AddComponent<MeshCollider>();
			collider.sharedMesh = meshFilter.mesh;
		} else
			Debug.Log("NO IT DOESNT");
		//collider.sharedMesh = myMesh;

	}

	private void scanFigure() {
		Vector3 originalPos = activeForm.gameObject.transform.position;
		Renderer rend = activeForm.gameObject.GetComponent<Renderer>();
		Bounds bounds = rend.bounds;

		Vector3 center = bounds.center;
		Vector3 extents = rend.bounds.extents;
		Vector3 begin = center - extents;

		float jumpsZ = 0.1f;
		float jumpsY = 0.1f;

		List<RaycastHit> listRaycast = new List<RaycastHit>();
		float limitZ = begin.z + (extents.z * 2);
		float limitY = begin.y + (extents.y * 2);
		float XFIJO = -2.0f;

		List<Int32> trianglesDetected = new List<Int32>();

		//collider of cube
		var colliderCube = activeForm.gameObject.GetComponent<MeshCollider>();
		for (float z = begin.z; z < limitZ; z += jumpsZ) {
			for (float y = begin.y; y < limitY; y += jumpsY) {
				RaycastHit hitN;
				Vector3 point = new Vector3(XFIJO, y, z);
				//Debug.Log(point);
				if (Physics.Raycast(point, Vector3.right, out hitN)) {
					triangleIndex = hitN.triangleIndex;
					//Debug.Log(hitN.distance);
					//Debug.Log(hitN.triangleIndex);

					if (!listRaycast.Exists(ExistsTriangleIndex)) {
						if (hitN.collider == colliderCube) {
							trianglesDetected.Add(hitN.triangleIndex);
							//Debug.Log("Si esta llenando la lista de triangulos");
						}
						listRaycast.Add(hitN);
						//Debug.Log("Encontro algo");
						//Debug.Log(hitN.triangleIndex);
					}
				}
			}
		}
		RaycastHit hit = listRaycast[0];
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null) {
			Debug.Log("Is returning");
			return;
		}
		Mesh mesh = colliderCube.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		Debug.Log("Vertices length: " + vertices.Length);



		List<Int32> convertidos = new List<Int32>();
		//Debug.Log(listRaycast.Count);

		for (int z = 0; z < vertices.Length; z++) {
			//Debug.Log("Vertex: " + z + " : " + vertices[z]);
		}

		Debug.Log("Division");
		foreach (RaycastHit r in listRaycast) {


			//Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
			//Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
			//Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
			//Debug.Log("Si entro");

			for (int hj = 0; hj < 3; hj++) {
				int vertexIndex = triangles[r.triangleIndex * 3 + hj];
				vertexIndexGlobal = vertexIndex;
				if (convertidos.Exists(HasInt)) {
					//Debug.Log("Si entra");
					//Vector3 v1 = vertices[vertexIndex];
					//Debug.Log("No entra porque ya lo hizo: " + v1);

					continue;
				}
				Vector3 v = vertices[vertexIndex];
				//Debug.Log(v);
				List<Int32> repetidos = new List<Int32>();
				repetidos.Add(vertexIndex);
				for (int k = 0; k < vertices.Length; k++) {

					if (k != vertexIndex && vector3Comparition(v, vertices[k])) {

						repetidos.Add(k);
					}
				}
				//Debug.Log("New");
				foreach (Int32 vIndex in repetidos) {
					Debug.Log("Vertex: " + vIndex + " : " + vertices[vIndex]);
					Vector3 vectorToTransform = activeForm.gameObject.transform.TransformPoint(vertices[vIndex]);
					vectorToTransform.x = -3f;
					//Debug.Log("Index: " + vIndex + "  ");
					Vector3 vectorTransformed = activeForm.gameObject.transform.InverseTransformPoint(vectorToTransform);
					vertices[vIndex] = vectorTransformed;
					//Debug.Log(vIndex);
					//Debug.Log("Vector real word: " + vectorToTransform + " Vector trasformed: " + vectorTransformed);
				}
				convertidos.AddRange(repetidos);
			}

			/*
			Debug.Log(vertices[triangles[hit.triangleIndex * 3 + 0]].x);
			Debug.Log(vertices[triangles[hit.triangleIndex * 3 + 1]].x);
			Debug.Log(vertices[triangles[hit.triangleIndex * 3 + 2]].x);
			
			Vector3 v0 = activeForm.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 0]]);
			Vector3 v1 = activeForm.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
			Vector3 v2 = activeForm.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
			Debug.Log(v0.x);
			Debug.Log(v1.x);
			Debug.Log(v2.x);
			v0.x = -2;
			v1.x = -2;
			v2.x = -2;
			v0=activeForm.gameObject.transform.InverseTransformPoint(v0);
			v1=activeForm.gameObject.transform.InverseTransformPoint(v1);
			v2=activeForm.gameObject.transform.InverseTransformPoint(v2);
			Debug.Log(v0.x);
			Debug.Log(v1.x);
			Debug.Log(v2.x);*/



			/*vertices[triangles[hit.triangleIndex * 3 + 0]].x = -0.2f;
			vertices[triangles[hit.triangleIndex * 3 + 1]].x = -0.2f;
			vertices[triangles[hit.triangleIndex * 3 + 2]].x = -0.2f;
			Debug.Log(vertices[triangles[hit.triangleIndex * 3 + 0]].x);
			Debug.Log(vertices[triangles[hit.triangleIndex * 3 + 1]].x);
			Debug.Log(vertices[triangles[hit.triangleIndex * 3 + 2]].x);*/

			/*
			Transform hitTransform = hit.collider.transform;
			p0 = hitTransform.TransformPoint(p0);
			p1 = hitTransform.TransformPoint(p1);
			p2 = hitTransform.TransformPoint(p2);
			Debug.DrawLine(p0, p1);
			Debug.DrawLine(p1, p2);
			Debug.DrawLine(p2, p0);	*/
		}
		for (int z = 0; z < vertices.Length; z++) {
			//Debug.Log("Vertex: " + z + " : " + vertices[z]);
		}
		/*
		for(int z = 0; z < vertices.Length; z++) {
			Debug.Log(vertices[z]);
		}*/
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		//colliderCube.sharedMesh = null;
		colliderCube.sharedMesh = mesh;



	}

	private bool vector3Comparition(Vector3 v1, Vector3 v2) {
		return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);
	}

	int triangleIndex;

	private bool ExistsTriangleIndex(RaycastHit r) {
		return r.triangleIndex == triangleIndex;
	}

	int vertexIndexGlobal;
	private bool HasInt(Int32 i) {
		return i == vertexIndexGlobal;
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

	public bool startPlaneSequence() {
		if (solveTryAttempts > 0 &&
			planeSequenceStatus == PlaneSequenceStatus.Idle &&
			xPlaneBehaviour.planeStatus == PlaneStatus.Idle &&
			zPlaneBehaviour.planeStatus == PlaneStatus.Idle
		) {
			planeSequenceStatus = PlaneSequenceStatus.MovingX;
			return true;
		}

		return false;
	}

	private void handlePlaneSequence() {
		if (planeSequenceStatus >= PlaneSequenceStatus.Ended) {
			if (planeSequenceStatus == PlaneSequenceStatus.Ended) {
				endCurrentLevel();
			} else {
				solveTryAttempts--;
				Debug.Log("Lose!");
			}

			planeSequenceStatus = PlaneSequenceStatus.Idle;
			issueMovePlane = false;

			if (solveTryAttempts > 0 && UIController.Instance) {
				UIController.Instance.toggleSolveButton(true);
				UIController.Instance.toggleTransformButtons(true);
			}
		}

		if (planeSequenceStatus > PlaneSequenceStatus.Idle && planeSequenceStatus < PlaneSequenceStatus.Ended) {
			if (planeSequenceStatus == PlaneSequenceStatus.MovingX) {
				if (xPlaneBehaviour.planeStatus == PlaneStatus.Idle) {
					if (!issueMovePlane) {
						issueMovePlane = true;
						xPlaneBehaviour.planeStatus = PlaneStatus.Move;
					}
				} else if (xPlaneBehaviour.planeStatus == PlaneStatus.Ended) {
					issueMovePlane = false;
					planeSequenceStatus = PlaneSequenceStatus.MovingZ;
				}
			}

			if (planeSequenceStatus == PlaneSequenceStatus.MovingZ) {
				if (zPlaneBehaviour.planeStatus == PlaneStatus.Idle) {
					if (!issueMovePlane) {
						issueMovePlane = true;
						zPlaneBehaviour.planeStatus = PlaneStatus.Move;
					}
				} else if (zPlaneBehaviour.planeStatus == PlaneStatus.Ended) {
					issueMovePlane = false;
					planeSequenceStatus = PlaneSequenceStatus.Ended;
				}
			}
		}
	}

	private void resetSolveTryAttempts() {
		solveTryAttempts = maxSolveTryAttempts;
	}

	public bool substractAndTestTransformAttempts() {
		return (--transformAttempts > 0);
	}

	public void resetTransformAttempts() {
		transformAttempts = maxTransformAttempts;
	}

	private void setAndInitCurrentLevel() {
		if (LevelController.Instance) {
			currentLevel = LevelController.Instance.getCurrentLevel();
		}

		if (currentLevel != null) {
			activeForm = Instantiate(currentLevel.form, formSpawn.position, Quaternion.identity, formSpawn);

			if (PlayerController.Instance) {
				PlayerController.Instance.setActiveForm(getActiveForm());
			}

			if (activeForm != null) {
				setFormToSolution();
				makeBothHoles();
				setFormToStartPosition();
				fadeInForm();
			}

			maxSolveTryAttempts = currentLevel.maxSolveAttempts;
			transformAttempts = currentLevel.maxTransformations;

			resetSolveTryAttempts();
			resetTransformAttempts();
		}
	}

	private void setFormToSolution() {
		activeForm.localPosition = currentLevel.position;
		activeForm.Rotate(currentLevel.rotation);

		// Temporal scale fix for plane colliders.
		activeForm.localScale = currentLevel.scale + new Vector3(scaleOffsetFix, scaleOffsetFix, scaleOffsetFix);
	}

	private void setFormToStartPosition() {
		activeForm.localPosition = currentLevel.startPosition;
		activeForm.Rotate(currentLevel.startRotation);
		activeForm.localScale = currentLevel.startScale;
	}

	private void fadeInForm() {
		if (this.formBehaviour) {
			this.formBehaviour.fadeIn(activeForm);
		}
	}

	private void endCurrentLevel() {
		Debug.Log("WIN");

		if (this.formBehaviour) {
			// this.formBehaviour.fadeOut(activeForm);
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
