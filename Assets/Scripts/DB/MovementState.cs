using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class MovementState {
    public Vector3 movement {get; set;}
    
    private class MovementStateJSON {
        public string movementJSON;        
    }

    public string ToJSON() {
        MovementStateJSON mJSON = new MovementStateJSON();
        mJSON.movementJSON = JsonUtility.ToJson(movement);
        return JsonUtility.ToJson(mJSON);
    }

    public static MovementState GetObjectFromJSON(string json) {
        MovementStateJSON mJSON = JsonUtility.FromJson<MovementStateJSON>(json);
        MovementState movementState = new MovementState();
        movementState.movement = JsonUtility.FromJson<Vector3>(mJSON.movementJSON);        
        return movementState;
    }
}