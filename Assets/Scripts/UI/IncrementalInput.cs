using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IncrementalInput : MonoBehaviour {

	[SerializeField]
	private TMP_InputField inputField;
	[SerializeField]
	private float angleIncrement = 45.0f;

    private GameController gameController;
    public GameObject gameObjectController;

    private void Awake() {
        this.gameController = gameObjectController.GetComponent<GameController>();
    }

    public void changeIncrementalInputRotation(int direction)
    {
        float currentAngle = (!string.IsNullOrEmpty(inputField.text)) ? float.Parse(inputField.text) : 0.0f;

        currentAngle += angleIncrement * direction;
        currentAngle = (currentAngle + 360.0f) % 360.0f;

        inputField.text = currentAngle.ToString();
    }
    
    public void incrementScaleX(int direction)
    {
        inputField.text = gameController.transformationsHelper.incrementScaleX(direction, getCurrentValue()).ToString();
    }

    public void incrementScaleY(int direction)
    {
        inputField.text = gameController.transformationsHelper.incrementScaleY(direction, getCurrentValue()).ToString();
    }

    public void incrementScaleZ(int direction)
    {
        inputField.text = gameController.transformationsHelper.incrementScaleZ(direction, getCurrentValue()).ToString();
    }

    public void incrementPositionX(int direction)
    {
        inputField.text = gameController.transformationsHelper.incrementPositionX(direction, getCurrentValue()).ToString();
    }

    public void incrementPositionY(int direction)
    {
        inputField.text = gameController.transformationsHelper.incrementPositionY(direction, getCurrentValue()).ToString();
    }

    public void incrementPositionZ(int direction)
    {
        inputField.text = gameController.transformationsHelper.incrementPositionZ(direction, getCurrentValue()).ToString();
    }

    private float getCurrentValue(float defualt = 0.0f) {
        return (inputField.text.Length == 0) ? defualt : (float.Parse(inputField.text));
    }
}
