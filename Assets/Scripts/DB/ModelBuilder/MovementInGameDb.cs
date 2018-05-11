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

public class MovementInGameDb  {

    /// It will return false in case the user already exists
    public static MovementInGame Insert(Vector3 movement, TypeTransform typeTranform, int numberTry, int gamePlayedId) { 
        MovementState movementState = new MovementState {
            movement = movement
        };

        MovementInGame movementInGame = new MovementInGame {
            GamePlayedId = gamePlayedId,
            NumberTry = numberTry,
            Movement = movementState.ToJSON(),
            TypeTranform = (int)typeTranform
        };

        var ds = new DataService();                         
        ds._connection.Insert(movementInGame);  

        List<MovementInGame> moevementInGameAll = GetAll();
		foreach(MovementInGame m in moevementInGameAll) {
			Debug.Log(m.ToString());
		}   

        return movementInGame;   
    }

    public static List<MovementInGame> GetAll(){
        var ds = new DataService ();
		return ds._connection.Table<MovementInGame>().ToList();
	}        

    public static GamePlayed GetById(int id){
        var ds = new DataService ();
		return ds._connection.Table<GamePlayed>().Where(g => g.Id == id).FirstOrDefault();
	}

}
