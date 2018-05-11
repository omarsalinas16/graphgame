using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

public class LoginController : MonoBehaviour {


	public InputField inputUsername;
	public InputField inputPassword;
	public GameObject menuController;

	public void Login() {
		string username = inputUsername.text;
		string password = inputPassword.text;

		if(username.Length == 0 || password.Length == 0) {
			return;
		}

		User user = UserDb.IsValidUser(username, password);
		if(user == null) {			
			UserDb.Insert(user);
		} else {						
			MenuController menuControllerScript = menuController.GetComponent<MenuController>();			
			menuControllerScript.GoToMainMenu(user);
		}
	}
	
	
}
