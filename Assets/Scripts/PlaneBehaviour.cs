using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaneStatus {
	Idle = 0,
	Move = 1,
	Return = 2,
	Ended = 3
}

public class PlaneBehaviour : MonoBehaviour {
	[SerializeField]
	private string formTag = "Form";

	public PlaneStatus planeStatus = PlaneStatus.Idle;

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
		if (planeStatus == PlaneStatus.Ended) {
			planeStatus = PlaneStatus.Idle;

			targetPosition = endPosition;
			currentLerpTime = 0.0f;
		}

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

	private void OnTriggerEnter(Collider other) {
		if (planeStatus == PlaneStatus.Move) {
			if (other.CompareTag(formTag)) {
				targetPosition = startPosition;
				planeStatus = PlaneStatus.Return;

				if (GameController.Instance) {
					GameController.Instance.planeSequenceStatus = PlaneSequenceStatus.Collided;
				}

				if (LookAtCamera.Instance) {
					LookAtCamera.Instance.shakeCamera(0.2f, 0.25f);
				}
			}
		}
	}
}
