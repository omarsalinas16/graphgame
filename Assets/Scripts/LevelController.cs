using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
	public static LevelController Instance { get; private set; }

	[SerializeField]
	private int _currentLevelIndex = 0;
	public int currentLevelIndex {
		get {
			return _currentLevelIndex;
		}

		set {
			int maxIndex = (levels != null && levels.Length > 0) ? levels.Length - 1 : 0;
			_currentLevelIndex = Mathf.Clamp(value, 0, maxIndex);
		}
	}

	[Header("Levels data")]
	public Level[] levels;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	private void Start() {
		// Read from an XML and set the levels array here?
	}

	public Level getCurrentLevel() {
		if (levels != null && levels.Length > 0 && currentLevelIndex >= 0) {
			return levels[currentLevelIndex];
		}

		return null;
	}

	public Level nextLevel() {
		currentLevelIndex++;

		return getCurrentLevel();
	}
}
