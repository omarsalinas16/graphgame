using UnityEngine;
using ModelFire;

using Assets.Scripts.DB.Firebase.ModelFire;

public class LevelController : MonoBehaviour {

	public static LevelController Instance { get; private set; }
	
	public static int currentLevelIndex {
		get ;
		set ;
	}

    public static LevelFire currentLevel
    {
        get;
        set;
    }

    //public GamePlayed ActualGame { get; set; }

	public ModelFire.UserFire user { get; set; }
	
	public Transform cube;	
	private const string CUBE = "Cube";

    private GameFireMaker gameFireMaker;

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
		if(user == null) {
			user = new ModelFire.UserFire {
				username = "Lalito", uid = "1"				
			};
			currentLevelIndex = 1;
		}
	}

	public Level getCurrentLevel() {
		Debug.Log("CurrentLevelIndex " + currentLevelIndex);
        /*ActualGame = GamePlayedDb.Insert(
			new GamePlayed {
				LevelId = currentLevelIndex,
				Solved = false
			}
		);*/
        //return LevelsBuilder.GetById(currentLevelIndex).ToLevel(this);	
        gameFireMaker = new GameFireMaker(user.uid, currentLevel.Id);
        return currentLevel.ToLevel(this);
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

    public void AddMovement(TYPE_MOVEMENT type, Vector3 movement) {
        gameFireMaker.AddMovement(type, movement);
    }

    public void Win() {
        gameFireMaker.End(true);
    }

    public void Lose() {
        gameFireMaker.End(false);
    }


}
