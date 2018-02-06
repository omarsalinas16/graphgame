using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance { get; private set; }

	public Transform activeForm = null;

	[Header("Settings")]
	[SerializeField]
	private float interpolationDuration = 2.0f;
	[SerializeField]
	private Ease interpolationEase = Ease.InOutCubic;

	[Header("Translation")]
	[SerializeField]
	private Vector3 translateLimitsMin;
	[SerializeField]
	private Vector3 translateLimitsMax;
	private Vector3 targetTranslate;

	[Header("Rotation")]
	private Quaternion targetRotation;
	private float _rotationAngleX = 0.0f;
	private float rotationAngleX {
		get { return _rotationAngleX; }
		set { _rotationAngleX = (value + 360.0f) % 360.0f; }
	}

	private float _rotationAngleY = 0.0f;
	private float rotationAngleY {
		get { return _rotationAngleY; }
		set { _rotationAngleY = (value + 360.0f) % 360.0f; }
	}

	[Header("Scale")]
	[SerializeField]
	private Vector2 scaleLimits;
	private Vector3 targetScale;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Start() {
		// Delegates and event suscriptions

		UIController uiController = UIController.Instance;

		if (uiController) {
			uiController.positionChangedEvent += addTargetTranslate;
			uiController.rotationChangedEvent += addTargetRotation;
			uiController.scalenChangedEvent += addTargetScale;

			uiController.resetGameEvent += initTargetTransforms;
		}
	}

	public void setActiveForm(Transform form) {
		activeForm = form;
		initTargetTransforms();
	}

	public void initTargetTransforms() {
		setTargetTranslate(0.0f, 0.0f, 0.0f);
		setTargetRotation(0.0f, 0.0f);
		setTargetScale(1.0f, 1.0f, 1.0f);
	}

	private void setTargetTranslate(float x, float y, float z) {
		targetTranslate.x = Mathf.Clamp(x, translateLimitsMin.x, translateLimitsMax.x);
		targetTranslate.y = Mathf.Clamp(y, translateLimitsMin.y, translateLimitsMax.y);
		targetTranslate.z = Mathf.Clamp(z, translateLimitsMin.z, translateLimitsMax.z);

		if (activeForm) {
			activeForm.DOLocalMove(targetTranslate, interpolationDuration).SetEase(interpolationEase);
		}
	}

	public void addTargetTranslate(float x, float y, float z) {
		x += targetTranslate.x;
		y += targetTranslate.y;
		z += targetTranslate.z;

		setTargetTranslate(x, y, z);
	}

	private void setTargetRotation(float x, float y) {
		rotationAngleX = x;
		rotationAngleY = y;

		targetRotation = Quaternion.Euler(rotationAngleX, rotationAngleY, 0.0f);

		if (activeForm) {
			activeForm.DORotateQuaternion(targetRotation, interpolationDuration).SetEase(interpolationEase);
		}
	}

	public void addTargetRotation(float x, float y) {
		x += rotationAngleX;
		y += rotationAngleY;

		setTargetRotation(x, y);
	}

	private void setTargetScale(float x, float y, float z) {
		targetScale.x = Mathf.Clamp(x, scaleLimits.x, scaleLimits.y);
		targetScale.y = Mathf.Clamp(y, scaleLimits.x, scaleLimits.y);
		targetScale.z = Mathf.Clamp(z, scaleLimits.x, scaleLimits.y);

		if (activeForm) {
			activeForm.DOScaleX(targetScale.x, interpolationDuration).SetEase(interpolationEase);
			activeForm.DOScaleY(targetScale.y, interpolationDuration).SetEase(interpolationEase);
			activeForm.DOScaleZ(targetScale.z, interpolationDuration).SetEase(interpolationEase);
		}
	}

	public void addTargetScale(float x, float y, float z) {
		x *= targetScale.x;
		y *= targetScale.y;
		z *= targetScale.z;

		setTargetScale(x, y, z);
	}
}
