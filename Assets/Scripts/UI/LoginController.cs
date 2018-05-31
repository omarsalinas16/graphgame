using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModelFire;
using SimpleFirebaseUnity;
using TMPro;

public class LoginController : MonoBehaviour {


	public InputField inputUsername;
	public InputField inputPassword;
    public GameObject error;
    //public GameObject menuController;

    public void Login() {
		string email = inputUsername.text;
		string password = inputPassword.text;

        firebaseLogin(email, password);
	}

    private void firebaseLogin(string email, string password)
    {        
        DbFire dbFire = new DbFire();
        this.error.SetActive(false);
        StartCoroutine(dbFire.IsUserValid(
            email, password,
            delegate(string uid) {
                Debug.Log(uid);
                GameObject parentUI = transform.parent.gameObject;
                MenuController menuControllerScript = parentUI.GetComponent<MenuController>();
                dbFire.GetUserFire(
                    delegate (UserFire u)
                    {
                        menuControllerScript.GoToMainMenu(u);
                    },
                    delegate (Firebase sender, FirebaseError error)
                    {
                        Debug.Log(error.Message);
                    }, uid
                );
                
            },
            delegate(string error) {
                Debug.Log(error);
                this.error.SetActive(true);
            }

        ));
    }

    /*
    private void localLogin(string username, string password) {
        if (username.Length == 0 || password.Length == 0)
        {
            return;
        }

        User user = UserDb.IsValidUser(username, password);
        if (user == null)
        {
            UserDb.Insert(user);
        }
        else
        {
            GameObject parentUI = transform.parent.gameObject;
            MenuController menuControllerScript = parentUI.GetComponent<MenuController>();
            menuControllerScript.GoToMainMenu(user);
        }
        inputUsername.text = "";
        inputPassword.text = "";
        //UserDb.Insert( new User {Username = username, Password = password});
    }*/


}
