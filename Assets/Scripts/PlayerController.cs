using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public static PlayerController Instance { get; private set; }

	private Transform _activeForm = null;
	public Transform activeForm {
		get {
			return _activeForm;
		}

		set {
			_activeForm = value;
		}
	}

	[Header("Settings")]
	[SerializeField]
	private float differenceThreshold = 0.001f;

	[Header("Translation")]
	[SerializeField]
	private bool smoothTranslate = false;
	[SerializeField]
	[Range(0.01f, 10.0f)]
	private float translateSmoothAmount = 1.0f;
	[SerializeField]
	private Vector3 translateLimitsMin;
	[SerializeField]
	private Vector3 translateLimitsMax;

	private Vector3 targetTranslate;
	private bool hasBeenTranslated = false;

	[Header("Rotation")]
	[SerializeField]
	private bool smoothRotation = false;
	[SerializeField]
	[Range(0.01f, 10.0f)]
	private float rotationSmoothAmount = 1.0f;

	private float rotationAngleX = 0.0f;
	private float rotationAngleY = 0.0f;
	private Quaternion targetRotation;
	private bool hasBeenRotated = false;

	[Header("Scale")]
	[SerializeField]
	private bool smoothScale = false;
	[SerializeField]
	[Range(0.01f, 10.0f)]
	private float scaleSmoothAmount = 1.0f;
	[SerializeField]
	private Vector2 scaleLimits;

	private Vector3 targetScale;
	private bool hasBeenScaled = false;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;
	}
	
	private void Update() {
		doTranslate();
		doRotation();
		doScaling();
	}

	public void setActiveForm(Transform form) {
		activeForm = form;
		
		hasBeenTranslated = false;
		hasBeenRotated = false;
		hasBeenScaled = false;

		initTargetTransforms();
	}

	public void initTargetTransforms() {
		targetTranslate = Vector3.zero;

		targetRotation = Quaternion.identity;
		rotationAngleX = 0.0f;
		rotationAngleY = 0.0f;

		targetScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	public void setTargetTranslate(float x, float y, float z) {
		targetTranslate.x += x;
		targetTranslate.y += y;
		targetTranslate.z += z;

		targetTranslate.x = Mathf.Clamp(targetTranslate.x, translateLimitsMin.x, translateLimitsMax.x);
		targetTranslate.y = Mathf.Clamp(targetTranslate.y, translateLimitsMin.y, translateLimitsMax.y);
		targetTranslate.z = Mathf.Clamp(targetTranslate.z, translateLimitsMin.z, translateLimitsMax.z);

		hasBeenTranslated = true;
	}

	private void doTranslate() {
		if (Time.timeScale < float.Epsilon || !hasBeenTranslated || !activeForm) {
			return;
		}

		if (smoothTranslate && translateSmoothAmount > 0.0f) {
			activeForm.localPosition = Vector3.Lerp(activeForm.localPosition, targetTranslate, translateSmoothAmount * Time.deltaTime);

			float difference = targetTranslate.magnitude - activeForm.localPosition.magnitude;

			if (Mathf.Abs(difference) <= differenceThreshold) {
				activeForm.localPosition = targetTranslate;
			}
		} else {
			activeForm.localPosition = targetTranslate;
		}
	}

	public void setTargetRotation(float x, float y) {
		rotationAngleX += x;

		if (rotationAngleX > 360.0f || rotationAngleX < -360.0f) {
			rotationAngleX = 0;
		}

		rotationAngleY += y;

		if (rotationAngleY > 360.0f || rotationAngleY < -360.0f) {
			rotationAngleY = 0.0f;
		}

		targetRotation = Quaternion.Euler(rotationAngleX, rotationAngleY, 0.0f);

		hasBeenRotated = true;
	}

	private void doRotation() {
		if (Time.timeScale < float.Epsilon || !hasBeenRotated || !activeForm) {
			return;
		}

		if (smoothRotation && rotationSmoothAmount > 0.0f) {
			activeForm.rotation = Quaternion.Slerp(activeForm.rotation, targetRotation, rotationSmoothAmount * Time.deltaTime);

			float difference = targetRotation.eulerAngles.magnitude - activeForm.rotation.eulerAngles.magnitude;

			if (Mathf.Abs(difference) <= differenceThreshold) {
				activeForm.rotation = targetRotation;
			}
		} else {
			activeForm.rotation = targetRotation;
		}
	}

	public void setTargetScale(float x, float y, float z) {
		targetScale.x = Mathf.Clamp(targetScale.x * x, scaleLimits.x, scaleLimits.y);
		targetScale.y = Mathf.Clamp(targetScale.y * y, scaleLimits.x, scaleLimits.y);
		targetScale.z = Mathf.Clamp(targetScale.z * z, scaleLimits.x, scaleLimits.y);

		hasBeenScaled = true;
	}

	private void doScaling() {
		if (Time.timeScale < float.Epsilon || !hasBeenScaled || !activeForm) {
			return;
		}

		if (smoothScale && scaleSmoothAmount > 0.0f) {
			activeForm.localScale = Vector3.Lerp(activeForm.localScale, targetScale, scaleSmoothAmount * Time.deltaTime);

			float difference = targetScale.magnitude - activeForm.localScale.magnitude;

			if (Mathf.Abs(difference) <= differenceThreshold) {
				activeForm.localScale = targetScale;
			}
		} else {
			activeForm.localScale = targetScale;
		}
	}
}
