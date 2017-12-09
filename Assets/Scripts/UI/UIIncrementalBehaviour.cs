using UnityEngine;
using UnityEngine.UI;

public class UIIncrementalBehaviour : MonoBehaviour {

	[SerializeField]
	private InputField inputField;
	[SerializeField]
	private float angleIncrement = 45.0f;

	public void changeIncrementalInput(int direction) {
		float currentAngle = (!string.IsNullOrEmpty(inputField.text)) ? float.Parse(inputField.text) : 0.0f;

		currentAngle += angleIncrement * direction;
		currentAngle = (currentAngle + 360.0f) % 360.0f;

		inputField.text = currentAngle.ToString();
	}
}
