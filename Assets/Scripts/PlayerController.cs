using UnityEngine;

public class PlayerController : MonoBehaviour {
	public static PlayerController Instance { get; private set; }

	public Transform activeForm = null;

	[Header("Settings")]
	[SerializeField]
	private float interpolationDuration = 2.0f;
	[SerializeField]
	private AnimationCurve easeCurve;

	[Header("Translation")]
	[SerializeField]
	private bool smoothTranslate = false;
	[SerializeField]
	private Vector3 translateLimitsMin;
	[SerializeField]
	private Vector3 translateLimitsMax;

	private Vector3 targetTranslate;
	private Vector3 lastTargetTranslate;
	private bool needsToTranslate = false;

	private float _translateInterpolation = 0.0f;
	private float translateInterpolation {
		get {
			return _translateInterpolation;
		}

		set {
			_translateInterpolation = Mathf.Clamp(value, 0.0f, interpolationDuration);
		}
	}

	[Header("Rotation")]
	[SerializeField]
	private bool smoothRotation = false;

	private float _rotationAngleX = 0.0f;
	private float rotationAngleX {
		get {
			return _rotationAngleX;
		}

		set {
			_rotationAngleX = value;

			if (_rotationAngleX < -360.0f || _rotationAngleX > 360.0f) {
				_rotationAngleX = 0.0f;
			}
		}
	}

	private float _rotationAngleY = 0.0f;
	private float rotationAngleY {
		get {
			return _rotationAngleY;
		}

		set {
			_rotationAngleY = value;

			if (_rotationAngleY < -360.0f || _rotationAngleY > 360.0f) {
				_rotationAngleY = 0.0f;
			}
		}
	}

	private Quaternion targetRotation;
	private Quaternion lastTargetRotation;
	private bool needsToRotate = false;

	private float _rotationInterpolation = 0.0f;
	private float rotationInterpolation {
		get {
			return _rotationInterpolation;
		}

		set {
			_rotationInterpolation = Mathf.Clamp(value, 0.0f, interpolationDuration);
		}
	}

	[Header("Scale")]
	[SerializeField]
	private bool smoothScale = false;
	[SerializeField]
	private Vector2 scaleLimits;

	private Vector3 targetScale;
	private Vector3 lastTargetScale;
	private bool needsToScale = false;

	private float _scaleInterpolation = 0.0f;
	private float scaleInterpolation {
		get {
			return _scaleInterpolation;
		}

		set {
			_scaleInterpolation = Mathf.Clamp(value, 0.0f, interpolationDuration);
		}
	}

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Start() {
		// Delegates and event suscriptions

		UIController uiController = UIController.Instance;

		uiController.positionChangedEvent += addTargetTranslate;
		uiController.rotationChangedEvent += addTargetRotation;
		uiController.scalenChangedEvent += addTargetScale;

		uiController.resetGameEvent += initTargetTransforms;
	}

	private void Update() {
		doTranslate();
		doRotation();
		doScaling();
	}

	public void setActiveForm(Transform form) {
		activeForm = form;

		initTargetTransforms();

		needsToTranslate = false;
		needsToRotate = false;
		needsToScale = false;
	}

	public void initTargetTransforms() {
		setTargetTranslate(0.0f, 0.0f, 0.0f);
		setTargetRotation(0.0f, 0.0f);
		setTargetScale(1.0f, 1.0f, 1.0f);
	}

	private void setTargetTranslate(float x, float y, float z) {
		lastTargetTranslate = targetTranslate;

		targetTranslate.x = Mathf.Clamp(x, translateLimitsMin.x, translateLimitsMax.x);
		targetTranslate.y = Mathf.Clamp(y, translateLimitsMin.y, translateLimitsMax.y);
		targetTranslate.z = Mathf.Clamp(z, translateLimitsMin.z, translateLimitsMax.z);

		needsToTranslate = true;
	}

	public void addTargetTranslate(float x, float y, float z) {
		x += targetTranslate.x;
		y += targetTranslate.y;
		z += targetTranslate.z;

		setTargetTranslate(x, y, z);
	}

	private void doTranslate() {
		if (Time.timeScale < float.Epsilon || !needsToTranslate || !activeForm) {
			return;
		}

		if (activeForm.localPosition == targetTranslate) {
			translateInterpolation = 0.0f;
			return;
		}

		if (smoothTranslate && interpolationDuration > 0.0f) {
			if (translateInterpolation <= interpolationDuration) {
				translateInterpolation += Time.deltaTime;

				float percentage = Mathf.Clamp01(translateInterpolation / interpolationDuration);
				activeForm.localPosition = Vector3.Lerp(lastTargetTranslate, targetTranslate, easeCurve.Evaluate(percentage));
			} else {
				activeForm.localPosition = targetTranslate;
				translateInterpolation = 0.0f;
			}
		} else {
			activeForm.localPosition = targetTranslate;
		}
	}

	private void setTargetRotation(float x, float y) {
		lastTargetRotation = targetRotation;

		rotationAngleX = x;
		rotationAngleY = y;

		targetRotation = Quaternion.Euler(rotationAngleX, rotationAngleY, 0.0f);

		needsToRotate = true;
	}

	public void addTargetRotation(float x, float y) {
		x += rotationAngleX;
		y += rotationAngleY;

		setTargetRotation(x, y);
	}

	private void doRotation() {
		if (Time.timeScale < float.Epsilon || !needsToRotate || !activeForm) {
			return;
		}

		if (activeForm.rotation == targetRotation) {
			rotationInterpolation = 0.0f;
			return;
		}

		if (smoothRotation && interpolationDuration > 0.0f) {
			if (rotationInterpolation <= interpolationDuration) {
				rotationInterpolation += Time.deltaTime;

				float percentage = Mathf.Clamp01(rotationInterpolation / interpolationDuration);
				activeForm.rotation = Quaternion.Slerp(lastTargetRotation, targetRotation, easeCurve.Evaluate(percentage));
			} else {
				activeForm.rotation = targetRotation;
				rotationInterpolation = 0.0f;
			}
		} else {
			activeForm.rotation = targetRotation;
		}
	}

	private void setTargetScale(float x, float y, float z) {
		lastTargetScale = targetScale;

		targetScale.x = Mathf.Clamp(x, scaleLimits.x, scaleLimits.y);
		targetScale.y = Mathf.Clamp(y, scaleLimits.x, scaleLimits.y);
		targetScale.z = Mathf.Clamp(z, scaleLimits.x, scaleLimits.y);

		needsToScale = true;
	}

	public void addTargetScale(float x, float y, float z) {
		x *= targetScale.x;
		y *= targetScale.y;
		z *= targetScale.z;

		setTargetScale(x, y, z);
	}

	private void doScaling() {
		if (Time.timeScale < float.Epsilon || !needsToScale || !activeForm) {
			return;
		}

		if (activeForm.localScale == targetScale) {
			scaleInterpolation = 0.0f;
			return;
		}

		if (smoothScale && interpolationDuration > 0.0f) {
			if (scaleInterpolation <= interpolationDuration) {
				scaleInterpolation += Time.deltaTime;

				float percentage = Mathf.Clamp01(scaleInterpolation / interpolationDuration);
				activeForm.localScale = Vector3.Lerp(lastTargetScale, targetScale, easeCurve.Evaluate(percentage));
			} else {
				activeForm.localScale = targetScale;
				scaleInterpolation = 0.0f;
			}
		} else {
			activeForm.localScale = targetScale;
		}
	}
}
