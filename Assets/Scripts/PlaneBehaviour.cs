using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour {
	[SerializeField]
	private string formTag = "Form";

	[SerializeField]
	private bool _allowMovement = false;
	public bool allowMovement {
		get {
			return _allowMovement;
		}

		set {
			_allowMovement = value;
		}
	}

	[Header("Plane Details")]
	[SerializeField]
	private float distanceToMove = 6.0f;
	[SerializeField]
	private float planeWidth = 0.3f;

	[Header("Movement")]
	[SerializeField]
	private Vector3 direction = Vector3.right;
	[SerializeField]
	private float lerpDuration;
	private float currentLerpTime = 0.0f;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	private bool hasStartedReturning = false;

	private bool _hasEnded = false;
	public bool hasEnded {
		get {
			return _hasEnded;
		}

		set {
			_hasEnded = value;
		}
	}

	private bool _hasCollided = false;
	public bool hasCollided {
		get {
			return _hasCollided;
		}

		set {
			_hasCollided = value;
		}
	}

	private void Start() {
		startPosition = transform.position;
		targetPosition = startPosition + direction * (distanceToMove - planeWidth);
	}

	private void Update() {
		if (hasEnded) return;

		if (allowMovement) {
			if (currentLerpTime >= lerpDuration) {
				transform.position = targetPosition;

				if (!hasStartedReturning) {
					targetPosition = startPosition;
					currentLerpTime = 0.0f;
					hasStartedReturning = true;
				} else {
					allowMovement = false;
					hasEnded = true;
				}
			} else {
				float percentaje = currentLerpTime / lerpDuration;

				transform.position = Vector3.Lerp(transform.position, targetPosition, percentaje);
				currentLerpTime += Time.deltaTime;
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag(formTag)) {
			hasCollided = true;
			hasEnded = true;

			if (LookAtCamera.Instance) {
				LookAtCamera.Instance.shakeCamera(0.2f, 0.25f);
			}

			Debug.Log("Collide!");
		}
	}
}
