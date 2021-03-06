﻿using UnityEngine;
using HoleMaker;
using UnityEngine.UI;

public enum PlaneSequenceStatus {
	Idle = 0,
	MovingX = 1,
	MovingZ = 2,
	Ended = 3,
	Collided = 4
}

[RequireComponent(typeof(PlayerController))]
public class GameController : MonoBehaviour {

	public static GameController Instance { get; private set; }

	public delegate void OnTryAttemptsChanged(int attempts);
	public event OnTryAttemptsChanged tryAttemptsChangedEvent;

	public delegate void OnTransformAttemptsChanged(int attempts);
	public event OnTransformAttemptsChanged transformAttemptsChangedEvent;

	[Header("Requirements")]
	[SerializeField]
	private Transform formSpawn;

	[Header("Points")]
	[SerializeField]
	private int maxSolveTryAttempts = 3;
	private int _solveTryAttempts;
	public int solveTryAttempts {
		get {
			return _solveTryAttempts;
		}

		set {
            //_solveTryAttempts = Mathf.Clamp(value, 0, maxSolveTryAttempts);
            _solveTryAttempts = value;

			if (tryAttemptsChangedEvent != null) {
				tryAttemptsChangedEvent(_solveTryAttempts);
			}
		}
	}

	[SerializeField]
	private int maxTransformAttempts = 5;
	private int _transformAttempts;
	public int transformAttempts {
		get {
			return _transformAttempts;
		}

		set {
            //_transformAttempts = Mathf.Clamp(value, 0, maxTransformAttempts);
            _transformAttempts = value;
            if (transformAttemptsChangedEvent != null) {
				transformAttemptsChangedEvent(_transformAttempts);
			}
		}
	}

	[Header("Planes")]
	[SerializeField]
	private GameObject xPlane;
	[SerializeField]
	private GameObject zPlane;

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

	[SerializeField]
	private FormBehaviour formBehaviour = new FormBehaviour();

	private PlayerController playerController;
	private LevelController levelController;
	private GameInputController uiController;

    public TransformationsHelper transformationsHelper { get; private set; }

    public Text resultText;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Start() {
		// Delegates and event suscriptions
		this.levelController = LevelController.Instance;
		this.playerController = PlayerController.Instance;
		this.uiController = GameInputController.Instance;

		this.uiController.resetGameEvent += resetTransformAttempts;

		if (xPlane) {
			xPlaneBehaviour = xPlane.GetComponent<PlaneBehaviour>();
		}

		if (zPlane) {
			zPlaneBehaviour = zPlane.GetComponent<PlaneBehaviour>();
		}

		setAndInitCurrentLevel();
	}

	private void Update() {
		handlePlaneSequence();
	}

	private void makeBothHoles() {
		HoleMaker.HoleMaker holeMaker = new HoleMaker.HoleMaker(this.activeForm.gameObject);
		holeMaker.makeXHole(xPlane);
		holeMaker.makeZHole(zPlane);
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
                // TODO: This has to be uncomment so the codes works again
				//endCurrentLevel();
			} else {
				solveTryAttempts--;
				Debug.Log("Lose!");
                resultText.text = "Lose!";
            }

			planeSequenceStatus = PlaneSequenceStatus.Idle;
			issueMovePlane = false;

            if (solveTryAttempts > 0 && this.uiController)
            {
                this.uiController.toggleSolveButton(true);
                this.uiController.toggleTransformButtons(true);
            }
            else {
                LevelController.Instance.Lose();                
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
		if (this.levelController) {
            currentLevel = this.levelController.getCurrentLevel();
            //currentLevel = new Level { form = this.levelController.GetTranformWithPrefabName("Cube") };
		}

		if (currentLevel != null) {
			activeForm = Instantiate(currentLevel.form, formSpawn.position, Quaternion.identity, formSpawn);
            transformationsHelper = new TransformationsHelper(activeForm, currentLevel.stepScale, currentLevel.stepTranslate);
			if (this.playerController) {
				this.playerController.setActiveForm(getActiveForm());
			}

			if (activeForm != null) {
				setFormToSolution();
				makeBothHoles();
				asignCollider();
				setFormToStartPosition();
				fadeInForm();
			}

			maxSolveTryAttempts = currentLevel.maxSolveAttempts;
			transformAttempts = currentLevel.maxTransformations;

			resetSolveTryAttempts();
			resetTransformAttempts();
		}
	}

	private void asignCollider() {
		// Set the collider
		MeshFilter meshFilter = activeForm.gameObject.GetComponent<MeshFilter>();
		var collider = activeForm.gameObject.AddComponent<MeshCollider>();
		collider.sharedMesh = meshFilter.mesh;				
		collider.convex = true;
		collider.isTrigger = true;
	}

	private void setFormToSolution() {
        /*if (this.formBehaviour != null) {
			this.formBehaviour.setFormToFinal(this.activeForm, this.currentLevel, this.scaleOffsetFix);
		}*/
        if (transformationsHelper != null)
        {
            transformationsHelper.setFormToSolvedState(this.currentLevel);
        }
    }

	private void setFormToStartPosition() {
        if (transformationsHelper != null) {
            transformationsHelper.setFormToStartState(this.currentLevel);
        }
		/*if (this.formBehaviour != null) {
			this.formBehaviour.setFormToStart(this.activeForm, this.currentLevel);
		}*/
	}

	private void fadeInForm() {
		if (this.formBehaviour != null) {
			this.formBehaviour.fadeIn(this.activeForm.gameObject);
		}
	}

	private void endCurrentLevel() {
		if (this.formBehaviour != null) {
			this.formBehaviour.fadeOut(this.activeForm.gameObject, false);
		}
        LevelController.Instance.Win();
        resultText.text = "Win!";
        Debug.Log("WIN");
	}

	public Transform searchActiveForm() {
		GameObject form = GameObject.FindGameObjectWithTag(formTag);
		return form ? form.transform : null;
	}

	public Transform getActiveForm() {
		return activeForm ? activeForm : searchActiveForm();
	}
}
