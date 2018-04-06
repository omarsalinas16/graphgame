using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Animator))]
public class SceneFader : MonoBehaviour {

	public static SceneFader Instance { get; private set; }

	public delegate void OnFadeOutCallback();

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Animator animator;
	[SerializeField]
	private string fadeOutBoolName = "FadeOut";

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Start() {
		if (this.canvasGroup == null) {
			this.canvasGroup = GetComponent<CanvasGroup>();
		}

		if (this.animator == null) {
			this.animator = GetComponent<Animator>();
		}
	}

	public void fadeOut(OnFadeOutCallback callback) {
		StartCoroutine(fadeOutRoutine(callback));
	}

	IEnumerator fadeOutRoutine(OnFadeOutCallback callback) {
		this.animator.SetBool(this.fadeOutBoolName, true);
		yield return new WaitUntil(() => this.canvasGroup.alpha == 1);

		callback();
	}
}
