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

	public void goToMainMenu(User user) {
		this.user = user;
		appearForm(Forms.MAINMENU);
	}

	public void logout() {
		this.user = null;
		appearForm(Forms.LOGIN);
	}

	public void appearForm(Forms f) {
		if(currentForm != null) {
			currentForm.SetActive(false);
		}
		GameObject formToAppear;
		formsDictionary.TryGetValue(f, out formToAppear);
		currentForm = formToAppear;
		formToAppear.SetActive(true);
	}

	void Awake() {
		fillFormsDictionary();
		appearForm(Forms.LEVELS);
	}



}
