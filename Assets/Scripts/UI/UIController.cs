using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

	public static UIController Instance { get; private set; }

	public delegate void OnGamePauseStateChanged(bool isPaused);
	public event OnGamePauseStateChanged onGamePauseStateChangedEvent;

	[SerializeField]
	private static string MENU_SCENE_NAME = "Menu";

	[SerializeField]
	private GameObject pauseMenu;

	private bool gamePaused = false;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
			toggleGamePause();
		}
	}

	public void toggleGamePause() {
		if (gamePaused) {
			setGamePauseState(false);
		} else {
			setGamePauseState(true);
		}
	}

	public void setGamePauseState(bool isPaused) {
		this.pauseMenu.SetActive(isPaused);
		Time.timeScale = isPaused ? 0f : 1f;
		this.gamePaused = isPaused;

		if (this.onGamePauseStateChangedEvent != null) {
			this.onGamePauseStateChangedEvent(this.gamePaused);
		}
	}

	public void loadMenuScene() {
		setGamePauseState(false);
		SceneFader sceneFader = SceneFader.Instance;

		if (sceneFader) {
			sceneFader.fadeOut(() => doLoadMenuScene());
		} else {
			doLoadMenuScene();
		}
	}

	private void doLoadMenuScene() {
		SceneManager.LoadScene(MENU_SCENE_NAME);
	}

	public void quitGame() {
		setGamePauseState(false);
		SceneFader sceneFader = SceneFader.Instance;

		if (sceneFader) {
			sceneFader.fadeOut(() => doQuitGame());
		} else {
			doQuitGame();
		}
	}

	private void doQuitGame() {
		Application.Quit();
		Debug.Log("Quitting game...");
	}
}
