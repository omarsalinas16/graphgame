using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// This is deprecated and replaced by TransformationsHelper
/// </summary>
public class PlayerController : MonoBehaviour {

	public static PlayerController Instance { get; private set; }

	public Transform activeForm = null;

	[Header("Settings")]
	[SerializeField]
	private float interpolationDuration = 2.0f;
	[SerializeField]
	private Ease interpolationEase = Ease.InOutCubic;

	[Header("Translation")]
	[SerializeField]
	private Vector3 translateLimitsMin;
	[SerializeField]
	private Vector3 translateLimitsMax;
	private Vector3 targetTranslate;

	[Header("Rotation")]
	private Quaternion targetRotation;
	private float _rotationAngleX = 0.0f;
	private float rotationAngleX {
		get { return _rotationAngleX; }
		set { _rotationAngleX = (value + 360.0f) % 360.0f; }
	}

	private float _rotationAngleY = 0.0f;
	private float rotationAngleY {
		get { return _rotationAngleY; }
		set { _rotationAngleY = (value + 360.0f) % 360.0f; }
	}

	private float _rotationAngleZ = 0.0f;
	private float rotationAngleZ {
		get { return _rotationAngleY; }
		set { _rotationAngleY = (value + 360.0f) % 360.0f; }
	}

	[Header("Scale")]
	[SerializeField]
	private Vector2 scaleLimits;
	private Vector3 targetScale;

    private Vector3 currentRotation = new Vector3(0,45,0);

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Start() {
		// Delegates and event suscriptions

		GameInputController uiController = GameInputController.Instance;

		if (uiController) {
			uiController.positionChangedEvent += addTargetTranslate;
			uiController.rotationChangedEvent += setTargetRotation;
			uiController.scalenChangedEvent += addTargetScale;

			uiController.resetGameEvent += initTargetTransforms;
		}
	}

	public void setActiveForm(Transform form) {
		activeForm = form;
		//initTargetTransforms();
	}

	public void initTargetTransforms() {
        
    }

    private void setFormToTranformation(Vector3 t, Vector3 s, List<Level.Rotation> rs) {        
        setTargetTranslate(t.x, t.y, t.z);
        foreach (Level.Rotation r in rs) {
            setTargetRotation(r.Value, r.Axis);
        }        
        setTargetScale(s.x, s.y, s.z);        
    }

    private void resetForm() {
        activeForm.position = new Vector3(0, 0, 0);        
        activeForm.rotation = Quaternion.Euler(0, 0, 0);
        activeForm.localScale = new Vector3(1, 1, 1);
    }

    private void setTargetTranslate(float x, float y, float z) {
		targetTranslate.x = Mathf.Clamp(x, translateLimitsMin.x, translateLimitsMax.x);
		targetTranslate.y = Mathf.Clamp(y, translateLimitsMin.y, translateLimitsMax.y);
		targetTranslate.z = Mathf.Clamp(z, translateLimitsMin.z, translateLimitsMax.z);

		if (activeForm) {
			activeForm.DOLocalMove(targetTranslate, interpolationDuration).SetEase(interpolationEase);
		}
	}

	public void addTargetTranslate(float x, float y, float z) {
		x += targetTranslate.x;
		y += targetTranslate.y;
		z += targetTranslate.z;

		setTargetTranslate(x, y, z);
	}

	private void setTargetRotation(float rotation, GameInputController.ROTATION_AXIS axis) {
        
		if (activeForm) {
            
            Vector3 currentRotation = new Vector3();
            Quaternion r;
            switch (axis) {
                case GameInputController.ROTATION_AXIS.X:
                    activeForm.RotateAround(activeForm.position, activeForm.right, rotation);
                    r = activeForm.rotation;
                    currentRotation = new Vector3(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);
                    //activeForm.RotateAround(activeForm.position, activeForm.right, -rotation);
                    break;
                case GameInputController.ROTATION_AXIS.Y:
                    activeForm.RotateAround(activeForm.position, activeForm.up, rotation);
                    r = activeForm.rotation;
                    currentRotation = new Vector3(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);
                    //activeForm.RotateAround(activeForm.position, activeForm.up, -rotation);
                    break;
                case GameInputController.ROTATION_AXIS.Z:
                    activeForm.RotateAround(activeForm.position, activeForm.forward, rotation);
                    r = activeForm.rotation;
                    currentRotation = new Vector3(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);
                    //activeForm.RotateAround(activeForm.position, activeForm.forward, -rotation);
                    break;
            }                        
            //activeForm.DORotate(currentRotation, interpolationDuration, RotateMode.FastBeyond360).SetEase(interpolationEase);
            // TODO: make the animarion great again
            
        }
	}	

	private void setTargetScale(float x, float y, float z) {
		targetScale.x = Mathf.Clamp(x, scaleLimits.x, scaleLimits.y);
		targetScale.y = Mathf.Clamp(y, scaleLimits.x, scaleLimits.y);
		targetScale.z = Mathf.Clamp(z, scaleLimits.x, scaleLimits.y);

		if (activeForm) {
			//activeForm.DOScaleX(targetScale.x, interpolationDuration).SetEase(interpolationEase);
			//activeForm.DOScaleY(targetScale.y, interpolationDuration).SetEase(interpolationEase);
			//activeForm.DOScaleZ(targetScale.z, interpolationDuration).SetEase(interpolationEase);
            activeForm.DOScale(targetScale, interpolationDuration).SetEase(interpolationEase);
        }
	}

	public void addTargetScale(float x, float y, float z) {
		x += targetScale.x;
		y += targetScale.y;
		z += targetScale.z;

		setTargetScale(x, y, z);
	}
}
