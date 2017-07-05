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
	
	public GameObject xPlane;
	public GameObject zPlane;
	private Vector3 newPosition;
	private Vector3 newScale;
	private Vector3 newRotation;
	
	public GameObject referenceCubes;
	
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
		putRandomTransform();
		makeWholeHoleX(xPlane);
		makeWholeHoleZ(zPlane);
		//makeHole(zPlane,Vector3.zero);
	}
	
	private void Update() {
		if ( Input.GetKey(KeyCode.DownArrow) ) {
			appearOrRotate(90,-90);
		}
		
		if ( Input.GetKey(KeyCode.LeftArrow) ) {
			appearOrRotate(0,90);
		}
	}
	
	private void appearOrRotate(int verAngle, int angle) {
		if(referenceCubes.transform.eulerAngles.y == verAngle) {
				referenceCubes.transform.Rotate(Vector3.up * angle);
				referenceCubes.SetActive(true);
		} else {
			if(referenceCubes.activeSelf) {
				referenceCubes.SetActive(false);
			} else  {
				referenceCubes.SetActive(true);
			}
		}
			
	}
	
	private void putRandomTransform() {
		//reposition();
		rescale();
		rotateRandom();
	}
	
	//beggin
	//Added by eduardo
	
	private void makeWholeHoleZ(GameObject objPlane) {
		makeHole(objPlane, Vector3.zero);
		float mov = 4.0f;
		while( mov < -4.0f ) { 
			makeHole(objPlane, new Vector3(0.0f,0.0f,mov));
			mov -= 0.3f;
		}
	}
	
	private void makeWholeHoleX(GameObject objPlane) {
		makeHole(objPlane, Vector3.zero);
		float mov = 4.0f;
		while( mov < -4.0f ) { 
			makeHole(objPlane, new Vector3(mov,0.0f,0.0f));
			mov -= 0.3f;
		}
	}
	
	private void makeHole(GameObject objPlane,Vector3 positionMov) {
		GameObject form = activeForm.gameObject;
		Vector3 originalPos = objPlane.transform.position;
		objPlane.transform.position = positionMov;
		
		Mesh m = CSG.Subtract(objPlane, form);
		
		objPlane.GetComponent<MeshFilter>().sharedMesh = m;
		
		objPlane.transform.position = originalPos;
		objPlane.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}
	
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
		GameObject form = activeForm.gameObject;
		form.transform.RotateAround(Vector3.up,putRandomPosition());
		form.transform.RotateAround(Vector3.right,putRandomPosition());
		form.transform.RotateAround(Vector3.forward,putRandomPosition());
	}
	
	private float putRandomPosition() {
		float randomN = Mathf.Round(Random.Range(-2.0f, 2.0f));
		if(randomN == 0)
			randomN = 0.5f;
		randomN = Mathf.Abs(randomN);
		return randomN;
	}
	//end
	
	

	public void initTransformAttempts() {
		transformAttempts = maxTransformAttempts;
	}

	private void spawnForm() {
		if (forms.Length > 0 && shapeIndex >= 0) {
			Form selectedForm = forms[shapeIndex];

			activeForm = Instantiate(selectedForm.shape, formSpawn.position, Quaternion.identity, formSpawn);
			
			if (activeForm) {
				if (FormBehaviour.Instance) {
					FormBehaviour.Instance.fadeIn(activeForm);
				}
			}

			if (PlayerController.Instance) {
				PlayerController.Instance.setActiveForm(getActiveForm());
			}

			initTransformAttempts();
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
