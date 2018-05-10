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

public class LevelsBuilder  {

    /// It will return false in case the user already exists
    public static bool Insert(LevelLocal level) {        
        var ds = new DataService();                 
        
        // TODO: check level doesnt repeat return false if it repeats
        var listLevels = ds._connection.Table<LevelLocal>().ToList();
        level.Name = "Level " + listLevels.Count() + " " + level.PrefabName;
        level.Id = listLevels.Count();
        ds._connection.Insert(level);
        return true;
    }

    public static List<LevelLocal> GetAll(){
        var ds = new DataService ();
		return ds._connection.Table<LevelLocal>().ToList();
	}        

    public static LevelLocal GetById(int id){
        var ds = new DataService ();
		return ds._connection.Table<LevelLocal>().Where(l => l.Id == id).FirstOrDefault();
	}

}
