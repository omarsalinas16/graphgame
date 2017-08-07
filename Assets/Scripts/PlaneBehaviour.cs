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
	[SerializeField]
	private float _currentLerpTime = 0.0f;
	private float currentLerpTime {
		get {
			return _currentLerpTime;
		}

		set {
			_currentLerpTime = Mathf.Clamp(value, 0.0f, lerpDuration);
		}
	}

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
		} else if (planeStatus > PlaneStatus.Idle) {
			if (currentLerpTime >= lerpDuration) {
				transform.position = targetPosition;

				if (planeStatus < PlaneStatus.Return) {
					planeStatus = PlaneStatus.Return;
					targetPosition = startPosition;
				} else {
					planeStatus = PlaneStatus.Ended;
					targetPosition = endPosition;
				}

				currentLerpTime = 0.0f;
			} else {
				currentLerpTime += Time.deltaTime;
				float percentage = Mathf.Clamp01(currentLerpTime / lerpDuration);

				transform.position = Vector3.Lerp(transform.position, targetPosition, percentage);
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (planeStatus == PlaneStatus.Move && other.CompareTag(formTag)) {
			planeStatus = PlaneStatus.Return;
			targetPosition = startPosition;

			if (GameController.Instance) {
				GameController.Instance.planeSequenceStatus = PlaneSequenceStatus.Collided;
			}

			if (LookAtCamera.Instance) {
				LookAtCamera.Instance.shakeCamera(0.2f, 0.25f);
			}
		}
	}
}
