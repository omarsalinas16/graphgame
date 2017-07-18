using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
	public static LookAtCamera Instance { get; private set; }

	[Header("Setup")]
	[SerializeField]
	private Camera mainCamera;
	private Quaternion startAngle;

	[Header("Turning")]
	[SerializeField]
	private bool allowInput = true;

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

	[Header("Cursor")]
	[SerializeField]
	private bool lockCursor = false;
	
	private float lookAngleX;
	private float lookAngleY;
	private Quaternion transformTargetRot;
	
	private float transformTargetScroll;
	
	[SerializeField]
	private GameObject lines;
	private GameObject instLines;
	
	[SerializeField]
	private Transform target;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;

		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = !lockCursor;

		transformTargetRot = transform.localRotation;

		if (mainCamera == null) {
			mainCamera = Camera.main;
		}

		startAngle = transform.rotation;

		lookAngleX = startAngle.eulerAngles.x;
		lookAngleY = startAngle.eulerAngles.y;

		transformTargetScroll = mainCamera.orthographicSize;
	}

	private void Update() {
		
		if (Input.GetKey("z") ) {
			if(!instLines) {
				instLines = Instantiate(lines,transform.position,Quaternion.identity);
			}
			transform.rotation = Quaternion.identity;
			return;
		}
		else if (Input.GetKey("x") ) {
			 Quaternion rotation = Quaternion.Euler(new Vector3(90, 0, 0));
			if(!instLines) {
				instLines = Instantiate(lines,transform.position,rotation);
			}
			transform.rotation = rotation;
			return;
		}
		else if (Input.GetKey("s") ) {
			 Quaternion rotation = Quaternion.Euler(new Vector3(0, -90, 0));
			if(!instLines) {
				instLines = Instantiate(lines,transform.position,rotation);
			}
			transform.rotation = rotation;
			return;
		}
		else if(instLines) {
			Destroy(instLines);
		}
		
		if (Time.timeScale < float.Epsilon)
			return;

		if (Input.GetButton("Fire1") && allowInput) {
			if (lockCursor) {
				Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
				Cursor.visible = !lockCursor;
			}

			handleRotationMovement();
		} else {
			if (lockCursor) {
				Cursor.lockState = lockCursor ? CursorLockMode.Confined : CursorLockMode.None;
				Cursor.visible = lockCursor;
			}

			if (turnSmoothing > 0 && transform.localRotation != transformTargetRot)
				transform.localRotation = Quaternion.Slerp(transform.localRotation, transformTargetRot, turnSmoothing * Time.deltaTime);
		}

		if (allowInput) {
			handleScrollZoom();
		}
	}

	private void OnDisable() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		allowInput = false;
	}

	private void handleRotationMovement() {
		if (Time.timeScale < float.Epsilon)
			return;

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

		transformTargetRot = Quaternion.Euler(lookAngleX, lookAngleY, 0f);

		if (turnSmoothing > 0) {
			transform.localRotation = Quaternion.Slerp(transform.localRotation, transformTargetRot, turnSmoothing * Time.deltaTime);
		} else {
			transform.localRotation = transformTargetRot;
		}
	}

	private void handleScrollZoom() {
		if (Time.timeScale < float.Epsilon || mainCamera == null)
			return;

		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			transformTargetScroll -= Input.GetAxis("Mouse ScrollWheel") * 1000 * scrollSpeed * Time.deltaTime;
			transformTargetScroll = Mathf.Clamp(transformTargetScroll, minZoom, maxZoom);
		}

		if (mainCamera.orthographicSize != transformTargetScroll) {
			if (scrollSmoothing > 0)
				mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, transformTargetScroll, scrollSmoothing * Time.deltaTime);
			else
				mainCamera.orthographicSize = transformTargetScroll;
		}
	}

	public void resetCameraPosition() {
		lookAngleX = startAngle.eulerAngles.x;
		lookAngleY = startAngle.eulerAngles.y;

		transformTargetRot = Quaternion.Euler(lookAngleX, lookAngleY, 0f);
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

			cameraShakePosition.x += Random.value * shakeAmount * 2 - shakeAmount;
			cameraShakePosition.z += Random.value * shakeAmount * 2 - shakeAmount;

			mainCamera.transform.position = cameraShakePosition;
		}
	}

	private void stopShake() {
		CancelInvoke("beginShake");
		mainCamera.transform.localPosition = lastCameraLocalPosition;
	}
}
