using UnityEngine;

using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine.Networking;
using ModelFire;

public class DbFire
{
    Firebase firebase;
    public DbFire() {        
        firebase = Firebase.CreateNew("schoolgraphgame.firebaseio.com", "jk4rjjoAH903ovhY3u9o6wIjqf0e3XYIeo6ievai");        
    }

    public void GetLevels(Action<Firebase, DataSnapshot> onSuccess, Action<Firebase, FirebaseError> onFail) {
        firebase.OnGetSuccess += onSuccess;
        firebase.OnGetFailed += onFail;
        firebase.Child("levels",true).GetValue();
    }

    // this can write
    /*public void IsUserValid(string email, string password, 
        Action<Firebase, DataSnapshot> PushOKHandler, Action<Firebase, FirebaseError> PushFailHandler) {


        //https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=[API_KEY]
        // API_KEY
        //AAAA5vF67ho:APA91bFOAGaS8a5pyrk8GVEs-xXuIl3-hZBfNQmvAeu9MOahna2WePup7GAM9ZctMrqyGkevxqPOjbIrB89OJYT2dvvEuASo3xhU1ZI_0tPjdBuoNP_hLTWofFRpaIvCtvIqckdmAlnW
        var credentials = new Dictionary<string,object>
        {
            { "email", email },
            { "password", password },
            { "returnSecureToken", true }
        };

        var API_KEY = "AAAA5vF67ho:APA91bFOAGaS8a5pyrk8GVEs - xXuIl3 - hZBfNQmvAeu9MOahna2WePup7GAM9ZctMrqyGkevxqPOjbIrB89OJYT2dvvEuASo3xhU1ZI_0tPjdBuoNP_hLTWofFRpaIvCtvIqckdmAlnW";
        var param = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=[" + API_KEY + "]";        

        string json = Json.Serialize(credentials);

        Debug.Log(json);

        firebase.OnPushSuccess += PushOKHandler;
        firebase.OnPushFailed += PushFailHandler;

        firebase.Push(json, true, param);

        

    }*/


    // this can write
    public IEnumerator IsUserValid(string email, string password, Action<string> onSuccess, Action<string> onFail)
    {



        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");


        var credentials = new Dictionary<string, object>
        {
            { "email", email },
            { "password", password },
            { "returnSecureToken", true }
        };

        // convert json string to byte
        var formData = System.Text.Encoding.UTF8.GetBytes(Json.Serialize(credentials));

        var POSTSigninUserURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=AIzaSyBD2RXoKupuCP6HxGThjJcqXaXu5joa3vQ";

        WWW www = new WWW(POSTSigninUserURL, formData, postHeader);

        yield return www; // Wait until the download is done
        if (www.error != null)
        {
            string result = System.Text.Encoding.UTF8.GetString(www.bytes);
            Debug.Log(result);
            onFail(result);
        }
        else
        {
            Debug.Log(www.text);
            var response = Json.Deserialize(www.text) as Dictionary<string, object>;
            var uid = (string)response["localId"];            
            onSuccess(uid);
        }
    }    

    public void GetUserFire(Action<UserFire> onSuccess, Action<Firebase, FirebaseError> onFail, string uid)
    {
        firebase.OnGetSuccess += delegate (Firebase sender, DataSnapshot snapshot) {
            string username = onUserObtained(sender, snapshot);
            onSuccess(new UserFire { username = username, uid = uid });
        };
        firebase.OnGetFailed += onFail;
        firebase.Child("users/" + uid, true).GetValue();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="snapshot"></param>
    /// <returns>The username</returns>
    public string onUserObtained(Firebase sender, DataSnapshot snapshot)
    {
        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        string username = (string)dict["username"];
        return username;
    }


    public class UserSiginResponse {
        public string kind { get; set; }
        public string idToken { get; set; }
        public string email { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
        public bool registered { get; set; }        

    }
    


}