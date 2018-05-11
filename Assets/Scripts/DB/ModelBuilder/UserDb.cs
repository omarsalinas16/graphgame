using SQLite4Unity3d;
using UnityEngine;
using Model;
using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;
using DatabaseL;

public class UserDb  {

    /// It will return false in case the user already exists
    public static bool Insert(User user) {        
        var ds = new DataService();  
        User userInDb = ds._connection.Table<User>().Where(x => x.Username == user.Username).FirstOrDefault();                   
        if(userInDb != null) return false;            
        ds._connection.Insert(user);

        List<User> usersAll = GetAll();
		foreach(User u in usersAll) {
			Debug.Log(u.ToString());
		}
        return true;
    }

    public static List<User> GetAll(){
        var ds = new DataService ();
		return ds._connection.Table<User>().ToList();
	}

    public IEnumerable<User> GetById(int id){
        var ds = new DataService ();
		return ds._connection.Table<User>().Where(x => x.Id == id);
	}

    /// It will return user if is valid if not it will return null, so be carefull
    public static User IsValidUser(string un, string p){
        var ds = new DataService ();
		User user = ds._connection.Table<User>().Where(x => x.Username == un).FirstOrDefault();      
        if(user == null) return null;  
        return (String.Compare(user.Password, p) == 0) ? user : null;
	}    

}
