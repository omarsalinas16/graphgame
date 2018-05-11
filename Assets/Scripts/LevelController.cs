using UnityEngine;
using Model;

public class LevelController : MonoBehaviour {

	public static LevelController Instance { get; private set; }
	
	public static int currentLevelIndex {
		get ;
		set ;
	}	

	public GamePlayed ActualGame { get; set; }

	public User user {get; set;}
	
	public Transform cube;	
	private const string CUBE = "Cube";

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
		Debug.Log("CurrentLevelIndex " + currentLevelIndex);
		ActualGame = GamePlayedDb.Insert(
			new GamePlayed {
				LevelId = currentLevelIndex,
				Solved = false
			}
		);
		return LevelsBuilder.GetById(currentLevelIndex).ToLevel(this);	
	}	

	public Level nextLevel() {
		currentLevelIndex++;
		return getCurrentLevel();
	}
	
	public string[] GetOptionsSelection() {
		return new string[] {
			CUBE
		};
	}

	public Transform GetTranformWithPrefabName(string prefabName) {
		switch(prefabName) {
			case CUBE:
				return cube;
		}
		return cube;
	}
}
