using UnityEngine;

public class GridBehaviour : MonoBehaviour {
	[SerializeField]
	private GameObject lines;
	private Quaternion lastCameraRotation;

	private bool needsToRevert = false;

	private LookAtCamera lookAtCamera;

	private void Start() {
		if (!lines) {
			lines = transform.GetChild(0).gameObject;
		}

		lines.SetActive(false);

		this.lookAtCamera = LookAtCamera.Instance;
	}

	private void Update() {
		if (Input.GetKeyDown("x")) {
			AxisXYZ.gridLinesColor = Color.red;
			setGridAndCameraRotation(90, 0, 0);
		} else if (Input.GetKeyDown("y")) {
			AxisXYZ.gridLinesColor = Color.green;
			setGridAndCameraRotation(0, -90, 0);
		} else if (Input.GetKeyDown("z")) {
			AxisXYZ.gridLinesColor = Color.blue;
			setGridAndCameraRotation(0, 0, 0);
		}

		if (Input.GetKeyUp("x") || Input.GetKeyUp("y") || Input.GetKeyUp("z")) {
			lines.SetActive(false);

			if (needsToRevert && this.lookAtCamera) {
				this.lookAtCamera.setTargetRotation(lastCameraRotation);
				this.lookAtCamera.allowInput = true;

				needsToRevert = false;
			}
		}
	}

	private void setGridAndCameraRotation(float x, float y, float z) {
		Quaternion rotation = Quaternion.Euler(x, y, z);

		lines.transform.rotation = rotation;

		if (this.lookAtCamera) {
			lastCameraRotation = this.lookAtCamera.getTargetRotation();
			this.lookAtCamera.setTargetRotation(rotation);

			this.lookAtCamera.allowInput = false;
		}

		lines.SetActive(true);
		needsToRevert = true;
	}
}
