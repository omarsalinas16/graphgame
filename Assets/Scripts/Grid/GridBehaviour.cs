using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour {
	[SerializeField]
	private GameObject lines;

	private void Start() {
		if (!lines) {
			lines = transform.GetChild(0).gameObject;
		}

		lines.SetActive(false);
	}

	private void Update() {
		if (Input.GetKey("z")) {
			rotate(new Vector3(0, 0, 0));
		} else if (Input.GetKey("x")) {
			rotate(new Vector3(90, 0, 0));
		} else if (Input.GetKey("s")) {
			rotate(new Vector3(0, -90, 0));
		} else {
			lines.SetActive(false);
			LookAtCamera.Instance.setRotatingFixed(false);
		}
	}

	private void rotate(Vector3 v) {
		Quaternion rotation = Quaternion.Euler(v);

		lines.SetActive(true);
		lines.transform.rotation = rotation;

		LookAtCamera.Instance.fixedRotation(rotation);
	}
}
