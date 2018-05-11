using SQLite4Unity3d;
using UnityEngine;
using Model;
using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using DatabaseL;
using System.Linq;

public class GamePlayedDb  {

    /// It will return false in case the user already exists
    public static GamePlayed Insert(GamePlayed gamePlayed) {        
        var ds = new DataService();                 
        
        gamePlayed.UserId = LevelController.Instance.user.Id;
        ds._connection.Insert(gamePlayed);

        List<GamePlayed> gamesPlayedAll = GetAll();
		foreach(GamePlayed g in gamesPlayedAll) {
			Debug.Log(g.ToString());
		}

        return gamePlayed;
    }

    /// It will return false in case the user already exists
    public static GamePlayed Update(GamePlayed gamePlayed) {        
        var ds = new DataService();                 
        
        ds._connection.Update(gamePlayed);

        List<GamePlayed> gamesPlayedAll = GetAll();
		foreach(GamePlayed g in gamesPlayedAll) {
			Debug.Log(g.ToString());
		}

        return gamePlayed;
    }

    public static List<GamePlayed> GetAll(){
        var ds = new DataService ();
		return ds._connection.Table<GamePlayed>().ToList();
	}        

    public static GamePlayed GetById(int id){
        var ds = new DataService ();
		return ds._connection.Table<GamePlayed>().Where(g => g.Id == id).FirstOrDefault();
	}

}
