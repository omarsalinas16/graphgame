using UnityEngine;
using DG.Tweening;

public class FormBehaviour : MonoBehaviour {
	public static FormBehaviour Instance { get; private set; }

	[Header("Fade Settings")]
	[SerializeField]
	private float fadeDuration = 0.5f;

	[Header("Alpha")]
	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float minAlpha = 0.0f;
	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float maxAlpha = 1.0f;

	[Header("Slide")]
	[SerializeField]
	private float minOffsetPosition = 0.0f;
	[SerializeField]
	private float maxOffsetPosition = -1.0f;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	public void fadeIn(Transform form) {
		fade(form, this.minOffsetPosition, this.maxAlpha);
	}

	public void fadeOut(Transform form) {
		fade(form, this.maxOffsetPosition, this.minAlpha);
	}

	private void fade(Transform form, float offset, float alpha) {
		Material material = form.GetComponent<Renderer>().material;

		float endOffset = getOffsetY(form.position, offset);
		Color endColor = getColorWithAlpha(material.color, alpha);

		form.DOMoveY(endOffset, fadeDuration);
		material.DOColor(endColor, fadeDuration);
	}

	private Color getColorWithAlpha(Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha); ;
	}

	private float getOffsetY(Vector3 position, float offset) {
		return position.y + offset;
	}
}
