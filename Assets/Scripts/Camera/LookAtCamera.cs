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

	[Header("Cursor")]
	[SerializeField]
	private bool lockCursor = false;

	[SerializeField]
	private float lookAngleX;
	[SerializeField]
	private float lookAngleY;
	private Quaternion transformTargetRot;
	
	private float transformTargetScroll;

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
		if (Time.timeScale < float.Epsilon)
			return;

		if (Input.GetButton("Fire1") && allowInput) {
			if (lockCursor) {
				Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
				Cursor.visible = !lockCursor;
			}

			HandleRotationMovement();
		} else {
			if (lockCursor) {
				Cursor.lockState = lockCursor ? CursorLockMode.Confined : CursorLockMode.None;
				Cursor.visible = lockCursor;
			}

			if (turnSmoothing > 0 && transform.localRotation != transformTargetRot)
				transform.localRotation = Quaternion.Slerp(transform.localRotation, transformTargetRot, turnSmoothing * Time.deltaTime);
		}

		if (allowInput) {
			HandleScrollZoom();
		}
	}

	private void OnDisable() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		allowInput = false;
	}

	private void HandleRotationMovement() {
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

	private void HandleScrollZoom() {
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

	public void ResetCameraPosition() {
		lookAngleX = startAngle.eulerAngles.x;
		lookAngleY = startAngle.eulerAngles.y;

		transformTargetRot = Quaternion.Euler(lookAngleX, lookAngleY, 0f);
	}
}
