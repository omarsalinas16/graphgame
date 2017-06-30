using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormBehaviour : MonoBehaviour {
	public static FormBehaviour Instance { get; private set; }

	[Header("Fade Settings")]
	[SerializeField]
	[Range(0.01f, 3.0f)]
	private float fadeDuration = 0.5f;
	[SerializeField]
	private float differenceThreshold = 0.001f;
	[SerializeField]
	private Vector2 offsetMinMax;

	[Header("Alpha")]
	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float startAlpha = 0.0f;
	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float endAlpha = 1.0f;

	[Header("Slide")]
	[SerializeField]
	private float startSlide = -1.0f;
	[SerializeField]
	private float endSlide = 0.0f;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;
	}

	public void fadeIn(Transform form) {
		Renderer renderer = form.GetComponent<Renderer>();

		float currentAlpha = startAlpha;
		float targetAlpha = endAlpha;

		float currentSlide = form.position.y + startSlide;
		float targetSlide = endSlide;

		float offsetTimer = Random.Range(offsetMinMax.x, offsetMinMax.y);

		setAlpha(renderer, currentAlpha);
		setSlide(form, currentSlide);

		StartCoroutine(fadeAndSlide(form, renderer, fadeDuration, offsetTimer, currentAlpha, targetAlpha, currentSlide, targetSlide));
	}

	public void fadeOut(Transform form) {
		Renderer renderer = form.GetComponent<Renderer>();

		float currentAlpha = endAlpha;
		float targetAlpha = startAlpha;

		float currentSlide = form.position.y + endSlide;
		float targetSlide = form.position.y - startSlide;

		float offsetTimer = 0;

		StartCoroutine(fadeAndSlide(form, renderer, fadeDuration, offsetTimer, currentAlpha, targetAlpha, currentSlide, targetSlide));
	}

	IEnumerator fadeAndSlide(Transform form, Renderer renderer, float fadeTimer, float offsetTimer, float currentAlpha, float targetAlpha, float currentSlide, float targetSlide) {
		if (Time.timeScale < float.Epsilon) {
			yield break;
		}

		if (offsetTimer < 0.0f) {
			if (fadeTimer > 0.0f) {
				float timeElapsed;

				if (fadeTimer > 0.0f) {
					timeElapsed = 1.0f - (fadeTimer / fadeDuration);
					timeElapsed = Mathf.Clamp(timeElapsed, 0.0f, 1.0f);
				} else {
					timeElapsed = 0.0f;
				}

				currentAlpha = interpolateValues(currentAlpha, targetAlpha, timeElapsed);
				setAlpha(renderer, currentAlpha);

				if (currentAlpha <= 0 && targetAlpha <= 0) {
					Destroy(form.gameObject);
					yield break;
				}

				currentSlide = interpolateValues(currentSlide, targetSlide, timeElapsed);
				setSlide(form, currentSlide);

				fadeTimer -= Time.deltaTime;
			} else {
				yield break;
			}
		} else {
			offsetTimer -= Time.deltaTime;
		}

		yield return new WaitForEndOfFrame();
		StartCoroutine(fadeAndSlide(form, renderer, fadeTimer, offsetTimer, currentAlpha, targetAlpha, currentSlide, targetSlide));
	}

	private void setAlpha(Renderer renderer, float currentAlpha) {
		if (!renderer) return;

		Color currentColor = renderer.material.color;
		renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentAlpha); ;
	}

	private void setSlide(Transform form, float currentSlide) {
		form.position = new Vector3(form.position.x, currentSlide, form.position.z);
	}

	private bool calculateDifference(float a, float b) {
		return Mathf.Abs(a - b) <= differenceThreshold;
	}

	private float interpolateValues(float current, float target, float time) {
		if (!current.Equals(target)) {
			current = Mathf.Lerp(current, target, time);

			if (calculateDifference(current, target)) {
				current = target;
			}
		}

		return current;
	}
}
