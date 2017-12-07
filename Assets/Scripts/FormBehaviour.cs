using UnityEngine;
using DG.Tweening;

public class FormBehaviour : MonoBehaviour {

	delegate void OnFadeCallback();

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

	private Material material;

	private void Awake() {
		material = GetComponent<Renderer>().material;
	}

	public void fadeIn() {
		fade(this.minOffsetPosition, this.maxAlpha);
	}

	public void fadeOut() {
		fade(this.maxOffsetPosition, this.minAlpha, () => Destroy(gameObject));
	}

	private void fade(float offset, float alpha, OnFadeCallback callback = null) {
		float endOffset = getOffsetY(transform.position, offset);
		Color endColor = getColorWithAlpha(material.color, alpha);

		transform.DOMoveY(endOffset, fadeDuration);
		Tweener tween = material.DOColor(endColor, fadeDuration);

		if (callback != null) {
			tween.OnComplete(() => callback());
		}
	}

	private Color getColorWithAlpha(Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha); ;
	}

	private float getOffsetY(Vector3 position, float offset) {
		return position.y + offset;
	}
}
