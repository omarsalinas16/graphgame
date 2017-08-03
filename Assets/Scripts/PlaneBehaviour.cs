using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaneStatus {
	Idle = 0,
	Move = 1,
	Return = 2,
	Ended = 3,
	Collided = 4
}

public class PlaneBehaviour : MonoBehaviour {
	[SerializeField]
	private string formTag = "Form";

	[SerializeField]
	private PlaneStatus _planeStatus = PlaneStatus.Idle;
	public PlaneStatus planeStatus {
		get {
			return _planeStatus;
		}

		set {
			_planeStatus = value;
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
	private Vector3 endPosition;
	private Vector3 targetPosition;

	private void Start() {
		startPosition = transform.position;
		endPosition = startPosition + direction * (distanceToMove - planeWidth);

		targetPosition = endPosition;
	}

	private void Update() {
		if (planeStatus > PlaneStatus.Idle && planeStatus < PlaneStatus.Ended) {
			if (currentLerpTime >= lerpDuration) {
				transform.position = targetPosition;

				if (planeStatus < PlaneStatus.Return) {
					targetPosition = startPosition;
					currentLerpTime = 0.0f;

					planeStatus = PlaneStatus.Return;
				} else {
					planeStatus = PlaneStatus.Ended;
				}
			} else {
				float percentaje = currentLerpTime / lerpDuration;

				transform.position = Vector3.Lerp(transform.position, targetPosition, percentaje);
				currentLerpTime += Time.deltaTime;
			}
		}
	}

	public void resetFlags() {
		planeStatus = PlaneStatus.Idle;

		targetPosition = endPosition;
		currentLerpTime = 0.0f;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag(formTag)) {
			planeStatus = PlaneStatus.Collided;

			if (LookAtCamera.Instance) {
				LookAtCamera.Instance.shakeCamera(0.2f, 0.25f);
			}

			Debug.Log("Collide!");
		}
	}
}
