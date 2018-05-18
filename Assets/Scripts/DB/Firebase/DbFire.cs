using UnityEngine;

using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine.Networking;


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
            var user = JsonUtility.FromJson<UserSiginResponse>(www.text);
            onSuccess(user.localId);
        }
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