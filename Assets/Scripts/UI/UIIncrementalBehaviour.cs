using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIncrementalBehaviour : MonoBehaviour {
	[SerializeField]
	private InputField inputField;
	[SerializeField]
	private float angleIncrement = 45.0f;

	public void changeIncrementalInput(int direction) {
		float currentAngle = 0.0f;

		if (!string.IsNullOrEmpty(inputField.text)) {
			currentAngle = float.Parse(inputField.text);
		}

		currentAngle += angleIncrement * direction;

		if (currentAngle > 360 || currentAngle < -360) {
			currentAngle = 0;
		}

		inputField.text = currentAngle.ToString();
	}
}
