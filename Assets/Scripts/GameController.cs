using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FormBehaviour))]
public class GameController : MonoBehaviour {
	public static GameController Instance { get; private set; }

	[Header("Requirements")]
	[SerializeField]
	private Transform formSpawn;

	[Header("Points")]
	[SerializeField]
	private int maxTransformAttempts = 5;
	private int _transformAttempts;
	public int transformAttempts {
		get {
			return _transformAttempts;
		}

		set {
			_transformAttempts = value;

			if (UIController.Instance) {
				UIController.Instance.setTransformAttempsLabel(_transformAttempts);
			}
		}
	}

	[Header("Shapes")]
	[SerializeField]
	private string formTag = "Form";
	[SerializeField]
	private Form[] forms;
	private Transform activeForm;

	private int shapeIndex = 0;

	private void Awake() {
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this;
	}

	private void Start() {
		spawnForm();
	}

	private void Update() {

	}

	public void initTransformAttempts() {
		transformAttempts = maxTransformAttempts;
	}

	private void spawnForm() {
		if (forms.Length > 0 && shapeIndex >= 0) {
			Form selectedForm = forms[shapeIndex];

			activeForm = Instantiate(selectedForm.shape, formSpawn.position, Quaternion.identity, formSpawn);

			if (activeForm) {
				if (FormBehaviour.Instance) {
					FormBehaviour.Instance.fadeIn(activeForm);
				}
			}

			if (PlayerController.Instance) {
				PlayerController.Instance.setActiveForm(getActiveForm());
			}

			initTransformAttempts();
		}
	}

	private void endCurrentLevel() {
		Debug.Log("WIN");

		if (FormBehaviour.Instance) {
			FormBehaviour.Instance.fadeOut(activeForm);
		}

		if (shapeIndex < forms.Length - 1) {
			shapeIndex++;
			spawnForm();
		}
	}

	public Transform searchActiveForm() {
		GameObject form = GameObject.FindGameObjectWithTag(formTag);
		return form ? form.transform : null;
	}

	public Transform getActiveForm() {
		return activeForm ? activeForm : searchActiveForm();
	}
}
