using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class TransformState {
    public Vector3 position {get; set;}
    public Vector3 rotation {get; set;}
    public Vector3 scale {get; set;}

    private class TransformStateJSON {
        public string positionJSON;
        public string rotationJSON;
        public string scaleJSON;
    }

    public string ToJSON() {
        TransformStateJSON tJSON = new TransformStateJSON();
        tJSON.positionJSON = JsonUtility.ToJson(position);
        tJSON.rotationJSON = JsonUtility.ToJson(rotation);
        tJSON.scaleJSON = JsonUtility.ToJson(scale);
        return JsonUtility.ToJson(tJSON);
    }

    public static TransformState GetObjectFromJSON(string json) {
        TransformStateJSON tJSON = JsonUtility.FromJson<TransformStateJSON>(json);
        TransformState transformState = new TransformState();
        transformState.position = JsonUtility.FromJson<Vector3>(tJSON.positionJSON);
        transformState.rotation = JsonUtility.FromJson<Vector3>(tJSON.rotationJSON);        
        transformState.scale = JsonUtility.FromJson<Vector3>(tJSON.scaleJSON);
        return transformState;
    }
}