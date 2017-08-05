using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
	public static LookAtCamera Instance { get; private set; }

	[Header("Setup")]
	[SerializeField]
	private Camera mainCamera;
	private Quaternion startAngle;
	
	public bool allowInput = true;

	[Header("Turning")]
	[SerializeField]
	[Range(1.0f, 10.0f)]
	private float turnSpeed = 1.0f;

	[SerializeField]
	private float turnSmoothing = 0.0f;
	[SerializeField]
	private bool invertXAxis = false;
	[SerializeField]
	private bool invertYAxis = false;

	[Header("Scrolling")]
	[SerializeField]
	[Range(0.0f, 5.0f)]
	private float scrollSpeed = 1.0f;
	[SerializeField]
	private float scrollSmoothing = 0.0f;
	[SerializeField]
	private float minZoom = 5.0f;
	[SerializeField]
	private float maxZoom = 10.0f;

	[Header("Camera Shake")]
	[SerializeField]
	private float shakeAmount = 2.0f;
	private Vector3 lastCameraLocalPosition;

	private float lookAngleX = 0.0f;
	private float lookAngleY = 0.0f;

	private Quaternion targetRotation;
	private float transformTargetScroll;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;
	}

	private void Start() {
		if (mainCamera == null) {
			mainCamera = Camera.main;
		}

		transformTargetScroll = mainCamera.orthographicSize;

		startAngle = transform.rotation;
		setTargetRotation(startAngle);
	}

	private void Update() {
		if (Time.timeScale < float.Epsilon) {
			return;
		}

		if (allowInput) {
			if (Input.GetButton("Fire1")) {
				handleRotationInput();
			}

			handleScrollZoom();
		}

		handleRotationMovement();
	}

	private void handleRotationInput() {
		float xInput = Input.GetAxis("Mouse Y") * (invertYAxis ? -1 : 1);
		float yInput = Input.GetAxis("Mouse X") * (invertXAxis ? 1 : -1);
		
		lookAngleX += xInput * turnSpeed;

		if (lookAngleX > 360 || lookAngleX < -360) {
			lookAngleX = 0;
		}
		
		lookAngleY += yInput * turnSpeed;

		if (lookAngleY > 360 || lookAngleY < -360) {
			lookAngleY = 0;
		}

		setTargetRotation(lookAngleX, lookAngleY, 0f);
	}

	private void handleRotationMovement() {
		if (turnSmoothing > 0) {
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		} else {
			transform.rotation = targetRotation;
		}
	}

	public Quaternion getTargetRotation() {
		return targetRotation;
	}

	public void setTargetRotation(Quaternion angle) {
		lookAngleX = angle.eulerAngles.x;
		lookAngleY = angle.eulerAngles.y;

		targetRotation = angle;
	}

	public void setTargetRotation(float x, float y, float z) {
		lookAngleX = x;
		lookAngleY = y;

		targetRotation = Quaternion.Euler(x, y, z);
	}

	private void handleScrollZoom() {
		if (mainCamera == null) {
			return;
		}

		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			transformTargetScroll -= Input.GetAxis("Mouse ScrollWheel") * 1000 * scrollSpeed * Time.deltaTime;
			transformTargetScroll = Mathf.Clamp(transformTargetScroll, minZoom, maxZoom);
		}

		if (mainCamera.orthographicSize != transformTargetScroll) {
			if (scrollSmoothing > 0) {
				mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, transformTargetScroll, scrollSmoothing * Time.deltaTime);
			} else {
				mainCamera.orthographicSize = transformTargetScroll;
			}
		}
	}

	public void resetCameraPosition() {
		setTargetRotation(startAngle);
	}

	public void shakeCamera(float amount, float duration) {
		shakeAmount = amount;
		lastCameraLocalPosition = mainCamera.transform.localPosition;

		InvokeRepeating("beginShake", 0.0f, 0.01f);
		Invoke("stopShake", duration);
	}

	private void beginShake() {
		if (shakeAmount > 0.0f) {
			Vector3 cameraShakePosition = mainCamera.transform.position;

			cameraShakePosition.x += Random.value * shakeAmount * 2.0f - shakeAmount;
			cameraShakePosition.z += Random.value * shakeAmount * 2.0f - shakeAmount;

			mainCamera.transform.position = cameraShakePosition;
		}
	}

	private void stopShake() {
		CancelInvoke("beginShake");
		mainCamera.transform.localPosition = lastCameraLocalPosition;
	}
}
