using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class FormBehaviour {

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

	public bool setFormToStart(Transform form, Level level) {        
		if (form && level != null) {
            Debug.Log("Set form to start: " + level.startPosition);
            form.localPosition = level.startPosition;

            Debug.Log("Level start rotation: " + level.startRotation);                        
            Vector3 sr = level.startRotation;
            form.rotation = Quaternion.Euler(sr.x, sr.y, sr.z);    
            
            form.localScale = level.startScale;
			return true;
		}

		return false;
	}

	public bool setFormToFinal(Transform form, Level level, float scaleOffsetFix) {
		if (form && level != null) {
			form.localPosition = level.position;
            
			form.Rotate(level.rotation);

			// Temporal scale fix for plane colliders.
			form.localScale = level.scale + new Vector3(scaleOffsetFix, scaleOffsetFix, scaleOffsetFix);

			return true;
		}

		return false;
	}

	public bool fadeIn(GameObject form) {
		return fade(form, this.maxAlpha);
	}

	public bool fadeOut(GameObject form, bool destroyOnEnd = true) {
		if (destroyOnEnd) {
			return fade(form, this.minAlpha, () => GameObject.Destroy(form));
		} else {
			return fade(form, this.minAlpha);
		}
	}

	private bool fade(GameObject form, float alpha, OnFadeCallback callback = null) {
		if (form == null) {
			return false;
		}

		Material material = form.GetComponent<Renderer>().material;
		Color endColor = getColorWithAlpha(material.color, alpha);
		
		Tweener tween = material.DOColor(endColor, fadeDuration);

		if (callback != null) {
			tween.OnComplete(() => callback());
		}

		return true;
	}

	private Color getColorWithAlpha(Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha); ;
	}
}
