using UnityEngine;
using DG.Tweening;

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

	[Header("Interpolation")]
	[SerializeField]
	private float interpolationDuration = 1.0f;
	[SerializeField]
	private Ease interpolationEase = Ease.OutCubic;
	[SerializeField]
	private float delayDuration = 0.2f;

	private float _currentDelayTime = 0.0f;
	private float currentDelayTime {
		get {
			return _currentDelayTime;
		}

		set {
			_currentDelayTime = Mathf.Clamp(value, 0.0f, delayDuration);
		}
	}

	private bool issuedInterpolation = false;
	private Vector3 startPosition;
	private Vector3 endPosition;

	private void Start() {
		updateStartPosition(this.transform.position);
	}

	private void Update() {
		if (planeStatus > PlaneStatus.Idle) {
			if (currentDelayTime <= 0.0f) {
				switch (planeStatus) {
					case PlaneStatus.Move:
						issueInterpolation(this.endPosition, PlaneStatus.Return);
						break;
					case PlaneStatus.Return:
						issueInterpolation(this.startPosition, PlaneStatus.Ended);
						break;
					case PlaneStatus.Ended:
						planeStatus = PlaneStatus.Idle;
						currentDelayTime = 0.0f;
						break;
				}
			} else {
				currentDelayTime -= Time.deltaTime;
			}
		}
	}

	private void issueInterpolation(Vector3 targetPosition, PlaneStatus targetStatus) {
		if (!issuedInterpolation) {
			issuedInterpolation = true;

			transform.DOMove(targetPosition, interpolationDuration)
				.SetEase(interpolationEase)
				.OnComplete(() => onInterpolationEnd(targetStatus));
		}
	}

	private void onInterpolationEnd(PlaneStatus nextStatus) {
		planeStatus = nextStatus;
		issuedInterpolation = false;
		currentDelayTime = delayDuration;
	}

	public void updateStartPosition(Vector3 position) {
		startPosition = position;
		endPosition = startPosition + direction * (distanceToMove - planeWidth);
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log("trigger");

		if (planeStatus == PlaneStatus.Move && other.CompareTag(formTag)) {
			Debug.Log("collided");

			planeStatus = PlaneStatus.Return;

			transform.DOKill();
			issuedInterpolation = false;
			currentDelayTime = 0.0f;

			if (GameController.Instance) {
				GameController.Instance.planeSequenceStatus = PlaneSequenceStatus.Collided;
			}

			if (LookAtCamera.Instance) {
				LookAtCamera.Instance.shakeCamera(0.3f, vibrato: 30);
			}
		}
	}
}
