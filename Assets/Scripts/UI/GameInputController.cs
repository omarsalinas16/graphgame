using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.DB.Firebase.ModelFire;

public class GameInputController : MonoBehaviour {

	public static GameInputController Instance { get; private set; }

	private GameController gameController;

	public delegate void OnPositionChanged(float x, float y, float z);
	public event OnPositionChanged positionChangedEvent;

	public delegate void OnRotationChanged(float x, float y, float z);
	public event OnRotationChanged rotationChangedEvent;

	public delegate void OnScaleChanged(float x, float y, float z);
	public event OnScaleChanged scalenChangedEvent;

	public delegate void OnResetGame();
	public event OnResetGame resetGameEvent;

	[SerializeField]
	private Button solveButton;

	[Header("Try Toggles")]
	[SerializeField]
	private Transform tryTogglesParent;
	[SerializeField]
	private GameObject tryTogglePreset;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI transformAttempts;

	[Header("Position Fields")]
	[SerializeField]
	private Button positionButton;
	[SerializeField]
	private TMP_InputField[] positionInputs = new TMP_InputField[3];

	[Header("Rotation Fields")]
	[SerializeField]
	private Button rotateButton;
	[SerializeField]
	private TMP_InputField[] rotationInputs = new TMP_InputField[3];

	[Header("Scale Fields")]
	[SerializeField]
	private Button scaleButton;
	[SerializeField]
	private TMP_InputField[] scaleInputs = new TMP_InputField[3];

	int countTries = 0;

    int maxSolveAttempts = 0;
    int maxTransformations = 0;

    public GameObject gameObjectController;


    private void Awake() {
        /*if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;

		// Delegates and event suscriptions

		this.gameController = GameController.Instance;*/

        Instance = this;

        this.gameController = gameObjectController.GetComponent<GameController>();

        if (gameController) {
			gameController.tryAttemptsChangedEvent += setTryAttemptsToggles;
			gameController.transformAttemptsChangedEvent += setTransformAttempsLabel;
		}

        maxSolveAttempts = LevelController.Instance.getCurrentLevel().maxSolveAttempts;
        maxTransformations = LevelController.Instance.getCurrentLevel().maxTransformations;
        //setTryAttemptsToggles(LevelController.Instance.getCurrentLevel().maxSolveAttempts);
        //setTransformAttempsLabel(LevelController.Instance.getCurrentLevel().maxTransformations);        
    }

	private void Update() {
		if (Input.GetKeyDown(KeyCode.T)) {
			setPosition();
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			setRotation();
		}

		if (Input.GetKeyDown(KeyCode.S)) {
			setScale();
		}
	}

	public void setTryAttemptsToggles(int amount) {
		List<Toggle> toggles = new List<Toggle>();
		GameObject toggleObject = null;
		Toggle toggleComponent = null;

		if (tryTogglesParent.childCount < amount) {
			foreach (Transform child in tryTogglesParent) {
				DestroyImmediate(child.gameObject);
			}

			for (int i = 0; i < amount; i++) {
				toggleObject = Instantiate(tryTogglePreset, tryTogglesParent);
				toggleComponent = toggleObject.GetComponent<Toggle>();

				toggles.Add(toggleComponent);
			}
		} else {
			foreach (Transform child in tryTogglesParent) {
				toggleComponent = child.GetComponent<Toggle>();
				toggles.Add(toggleComponent);
			}
		}

		foreach (Toggle toggle in toggles) {
			if (amount-- > 0) {
				toggle.isOn = true;
			} else {
				toggle.isOn = false;
			}
		}
	}

	public void setTransformAttempsLabel(int amount) {
		if (transformAttempts) {
			transformAttempts.text = (amount >= 0 ? amount : 0).ToString();
		}
	}

	public void runSolveTry() {
		if (gameController && gameController.solveTryAttempts > 0) {
			if (gameController.startPlaneSequence()) {
				toggleSolveButton(false);
				toggleTransformButtons(false);
			}
		}
	}

	public void toggleSolveButton(bool isActive) {
		if (solveButton) {
			solveButton.interactable = isActive;
		}
	}

	public void toggleTransformButtons(bool isActive) {
		if (positionButton) {
			positionButton.interactable = isActive;
		}

		if (rotateButton) {
			rotateButton.interactable = isActive;
		}

		if (scaleButton) {
			scaleButton.interactable = isActive;
		}
	}

	private float readInputValue(TMP_InputField input, float defaultValue = 0.0f) {
		string inputText = input.text;
		input.text = null;

		return (!string.IsNullOrEmpty(inputText)) ? float.Parse(inputText) : defaultValue;
	}

	public void setPosition() {
		float[] inputValues = positionInputs.Select(input => readInputValue(input)).ToArray();

		if (gameController.substractAndTestTransformAttempts()) {
			if (positionChangedEvent != null && inputValues.Length == positionInputs.Length) {
				positionChangedEvent(inputValues[0], inputValues[1], inputValues[2]);
                saveMovement(TYPE_MOVEMENT.TRANSLATE, inputValues);
            }
		}
	}

	public void setRotation() {
		float[] inputValues = rotationInputs.Select(input => readInputValue(input)).ToArray();

		if (gameController.substractAndTestTransformAttempts()) {
			if (rotationChangedEvent != null && inputValues.Length == rotationInputs.Length) {
				for(int i = 0; i < inputValues.Length; i++) {
					Debug.Log("Input values "+i+": " + inputValues[i]);
				}				
				rotationChangedEvent(inputValues[0], inputValues[1], inputValues[2]);
                saveMovement(TYPE_MOVEMENT.ROTATE, inputValues);
            }
		}

		
	}

	public void setScale() {
		float[] inputValues = scaleInputs.Select(input => readInputValue(input, 1.0f)).ToArray();

		if (gameController.substractAndTestTransformAttempts()) {
			if (scalenChangedEvent != null && inputValues.Length == scaleInputs.Length) {
				scalenChangedEvent(inputValues[0], inputValues[1], inputValues[2]);
                saveMovement(TYPE_MOVEMENT.SCALE, inputValues);
			}
		}

		
	}

    /*private void saveMovement(float[] inputValues, TypeTransform typeTransform) {
		Vector3 movement;
		
		movement = new Vector3(inputValues[0], inputValues[1], inputValues[2]);
		MovementInGameDb.Insert(movement, typeTransform, countTries, LevelController.Instance.ActualGame.Id);
		countTries++;
	}*/

    private void saveMovement(TYPE_MOVEMENT t, float[] inputValues)
    {
        Vector3 movement;
        movement = new Vector3(inputValues[0], inputValues[1], inputValues[2]);
        LevelController.Instance.AddMovement(t, movement);        
    }

    public void resetAll() {
		if (resetGameEvent != null) {
			resetGameEvent();
		}
	}
}
