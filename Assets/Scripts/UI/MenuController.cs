using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

public class MenuController : MonoBehaviour {	

	public enum Forms {
		LOGIN, MAINMENU, LEVELS, CREATE_LEVELS, HISTORY
	}

	private GameObject currentForm;
	private User user;

	public GameObject loginForm;
	public GameObject mainMenuForm;
	public GameObject levelsForm;
	public GameObject historyForm;
	public GameObject createLevelsForm;	

	Dictionary<Forms, GameObject> formsDictionary = new Dictionary<Forms, GameObject>();

	private void fillFormsDictionary() {
		formsDictionary.Add(Forms.LOGIN, loginForm);
		formsDictionary.Add(Forms.MAINMENU, mainMenuForm);
		formsDictionary.Add(Forms.LEVELS, levelsForm);		
		formsDictionary.Add(Forms.HISTORY, historyForm);
		formsDictionary.Add(Forms.CREATE_LEVELS, createLevelsForm);		
	}

	public void GoToMainMenu(User user) {
		LevelController.Instance.user = user;		
		appearForm(Forms.MAINMENU);
		currentForm.GetComponent<MainMenuController>().SetUser(user);
	}

	public void Logout() {
		LevelController.Instance.user = null;
		appearForm(Forms.LOGIN);		
	}

	public void GoToMainMenu() {		
		appearForm(Forms.MAINMENU);
	}	

	public void appearForm(Forms f) {
		if(currentForm != null) {
			Destroy(currentForm);
		}
		GameObject formToAppear;
		formsDictionary.TryGetValue(f, out formToAppear);		
		currentForm = Instantiate(formToAppear, transform);		
	}	

	void Awake() {
		fillFormsDictionary();
		appearForm(Forms.LOGIN);
	}



}
