using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour {

	[SerializeField]
	private static string MAIN_SCENE_NAME = "Main";

	[SerializeField]
	private GameObject optionsMenu;

	[SerializeField]
	private TMP_Text userName;

	public void loadMainScene() {
		SceneFader sceneFader = SceneFader.Instance;

		if (sceneFader) {
			sceneFader.fadeOut(() => doLoadMainScene());
		} else {
			doLoadMainScene();
		}
	}

	private void doLoadMainScene() {
		SceneManager.LoadScene(MAIN_SCENE_NAME);
	}

	public void quitGame() {
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
