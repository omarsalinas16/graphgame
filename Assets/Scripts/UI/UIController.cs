using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	public static UIController Instance { get; private set; }

	[Header("Try Toggles")]
	[SerializeField]
	private Transform tryTogglesParent;
	[SerializeField]
	private GameObject tryTogglePreset;

	[Header("Labels")]
	[SerializeField]
	private Text transformAttemptsLabel;

	[Header("Position Fields")]
	[SerializeField]
	private InputField xPosition;
	[SerializeField]
	private InputField yPosition;
	[SerializeField]
	private InputField zPosition;

	[Header("Rotation Fields")]
	[SerializeField]
	private InputField xRotation;
	[SerializeField]
	private InputField yRotation;
	[SerializeField]
	private InputField zRotation;

	[Header("Scale Fields")]
	[SerializeField]
	private InputField xScale;
	[SerializeField]
	private InputField yScale;
	[SerializeField]
	private InputField zScale;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;
	}

	public void setSolveTryAttempts(int amount) {
		int i = 0;

		if (tryTogglesParent.childCount < amount) {
			foreach (Transform child in tryTogglesParent) {
				DestroyImmediate(child.gameObject);
			}

			for (i = 0; i < amount; i++) {
				Instantiate(tryTogglePreset, tryTogglesParent);
			}
		}

		if (tryTogglesParent.childCount <= 0) {
			return;
		}

		Toggle tryToggle;

		for (i = 0; i < tryTogglesParent.childCount; i++) {
			tryToggle = tryTogglesParent.GetChild(i).GetComponent<Toggle>();

			if (amount > 0) {
				if (tryToggle) {
					tryToggle.isOn = true;
				}

				amount--;
			} else {
				if (tryToggle) {
					tryToggle.isOn = false;
				}
			}
		}
	}

	public void setTransformAttempsLabel(int amount) {
		if (transformAttemptsLabel) {
			transformAttemptsLabel.text = (amount >= 0 ? amount : 0).ToString();
		}
	}

	public void runSolveTry() {
		if (GameController.Instance) {
			GameController.Instance.startPlaneSequence();
		}
	}

	public void setPosition() {
		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;

		if (!string.IsNullOrEmpty(xPosition.text)) {
			x = float.Parse(xPosition.text);
		}

		if (!string.IsNullOrEmpty(yPosition.text)) {
			y = float.Parse(yPosition.text);
		}

		if (!string.IsNullOrEmpty(zPosition.text)) {
			z = float.Parse(zPosition.text);
		}

		xPosition.text = null;
		yPosition.text = null;
		zPosition.text = null;

		if (GameController.Instance.transformAttempts > 0) {
			GameController.Instance.transformAttempts--;

			if (PlayerController.Instance) {
				PlayerController.Instance.setTargetTranslate(x, y, z);
			}
		}
	}

	public void setRotation() {
		float x = 0.0f;
		float y = 0.0f;

		if (!string.IsNullOrEmpty(xRotation.text)) {
			x = float.Parse(xRotation.text);
		}

		if (!string.IsNullOrEmpty(yRotation.text)) {
			y = float.Parse(yRotation.text);
		}

		xRotation.text = null;
		yRotation.text = null;
		zRotation.text = null;

		if (GameController.Instance.transformAttempts > 0) {
			GameController.Instance.transformAttempts--;

			if (PlayerController.Instance) {
				PlayerController.Instance.setTargetRotation(x, y);
			}
		}
	}

	public void setScale() {
		float x = 1.0f;
		float y = 1.0f;
		float z = 1.0f;

		if (!string.IsNullOrEmpty(xScale.text)) {
			x = float.Parse(xScale.text);
		}

		if (!string.IsNullOrEmpty(yScale.text)) {
			y = float.Parse(yScale.text);
		}

		if (!string.IsNullOrEmpty(zScale.text)) {
			z = float.Parse(zScale.text);
		}

		xScale.text = null;
		yScale.text = null;
		zScale.text = null;

		if (GameController.Instance.transformAttempts > 0) {
			GameController.Instance.transformAttempts--;

			if (PlayerController.Instance) {
				PlayerController.Instance.setTargetScale(x, y, z);
			}
		}
	}

	public void resetAll() {
		if (GameController.Instance) {
			GameController.Instance.initTransformAttempts();
		}

		if (PlayerController.Instance) {
			PlayerController.Instance.initTargetTransforms();
		}

		if (LookAtCamera.Instance) {
			LookAtCamera.Instance.resetCameraPosition();
		}
	}
}
