using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Model;
public class MainMenuController : MonoBehaviour {

	[SerializeField]
	private static string MAIN_SCENE_NAME = "Main";

	[SerializeField]
	private GameObject optionsMenu;

	[SerializeField]
	private TMP_Text userName;

	public void Start() {
		User user = LevelController.Instance.user;
		if(user != null) {
			SetUser(user);
		}
	}

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

	public void SetUser(User user) {
		userName.text = user.Username;
	}

	public void Logout() {
		GameObject parentUI = transform.parent.gameObject;
		MenuController menuControllerScript = parentUI.GetComponent<MenuController>();			
		menuControllerScript.Logout();
	}

	public void AppearLevels() {
		Appear(MenuController.Forms.LEVELS);
	}

	public void AppearCreateLevels() {
		Appear(MenuController.Forms.CREATE_LEVELS);
	}

	private void Appear(MenuController.Forms form) {
		GameObject parentUI = transform.parent.gameObject;
		MenuController menuControllerScript = parentUI.GetComponent<MenuController>();			
		menuControllerScript.appearForm(form);
	}
}
