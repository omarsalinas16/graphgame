using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour {
	[SerializeField]
	private GameObject lines;
	private Quaternion lastCameraRotation;

	private bool needsToRevert = false;

	private void Start() {
		if (!lines) {
			lines = transform.GetChild(0).gameObject;
		}

		lines.SetActive(false);
	}

	private void Update() {
		if (Input.GetKeyDown("x")) {
			setGridAndCameraRotation(90, 0, 0);
		}

		if (Input.GetKeyDown("y")) {
			setGridAndCameraRotation(0, -90, 0);
		}

		if (Input.GetKeyDown("z")) {
			setGridAndCameraRotation(0, 0, 0);
		}

		if (Input.GetKeyUp("x") || Input.GetKeyUp("y") || Input.GetKeyUp("z")) {
			lines.SetActive(false);

			if (needsToRevert && LookAtCamera.Instance) {
				LookAtCamera.Instance.setTargetRotation(lastCameraRotation);
				LookAtCamera.Instance.allowInput = true;

				needsToRevert = false;
			}
		}
	}

	private void setGridAndCameraRotation(float x, float y, float z) {
		Quaternion rotation = Quaternion.Euler(x, y, z);

		lines.transform.rotation = rotation;

		if (LookAtCamera.Instance) {
			lastCameraRotation = LookAtCamera.Instance.getTargetRotation();
			LookAtCamera.Instance.setTargetRotation(rotation);

			LookAtCamera.Instance.allowInput = false;
		}

		lines.SetActive(true);
		needsToRevert = true;
	}
}
