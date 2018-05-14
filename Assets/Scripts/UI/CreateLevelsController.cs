using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;
using System;
using TMPro;
using System.Linq;

public class CreateLevelsController : MonoBehaviour {

	//Create a List of new Dropdown options
    List<string> dropOptions = new List<string> { "Cube" };
    //This is the Dropdown
    public Dropdown dropdown;

	public InputField inputStartPos;
	public InputField inputStartRot;
	public InputField inputStartScale;

	public InputField inputSolvedPos;
	public InputField inputSolvedRot;
	public InputField inputSolvedScale;

	public InputField inputMaxAttemps;
	public InputField inputMaxTransformations;

	// Use this for initialization
	void Start () {
		//Clear the old options of the Dropdown menu
        dropdown.ClearOptions();
        //Add the options created in the List above
		LevelController levelController = LevelController.Instance;

        dropdown.AddOptions(levelController.GetOptionsSelection().OfType<string>().ToList());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateLevel() {
		// TODO: appear a dialog where it says it was succesfull or not
		LevelsBuilder.Insert(GetLevel());
		List<LevelLocal> levelLocals = LevelsBuilder.GetAll();
		foreach(LevelLocal l in levelLocals) {
			Debug.Log(l.ToString());
		}
	}

	LevelLocal GetLevel() {
		string startPos = inputStartPos.text;
		string startRot = inputStartRot.text;
		string startScale = inputStartScale.text;

		string solvedPos = inputSolvedPos.text;
		string solvedRot = inputSolvedRot.text;
		string solvedScale = inputSolvedScale.text;

		string prefabName = dropOptions[dropdown.value];

		int maxAttemps = 0;
		Int32.TryParse(inputMaxAttemps.text, out maxAttemps);
		int maxTransformations = 0;
		Int32.TryParse(inputMaxTransformations.text, out maxTransformations);

		return new LevelLocal {			
			StartState = GetTransformStateFromStrings(startPos, startRot, startScale).ToJSON(),
			SolvedState = GetTransformStateFromStrings(solvedPos, solvedRot, solvedScale).ToJSON(),
			PrefabName = prefabName,
			MaxSolveAttemps = maxAttemps,
			MaxTransformations = maxTransformations
		};
	}

	/// This returns a transform state base on three string: position, rotation and scale
	private TransformState GetTransformStateFromStrings(string pos, string rot, string scale) {
		return new TransformState {
			position = StringToVector3(pos),
			rotation = StringToVector3(rot),
			scale = StringToVector3(scale)
		};
	}


	/// This string has to be in the form of "x,y,z" where x,y and z are numbers
	private Vector3 StringToVector3(string strVector) {
		string[] startPosStrings = strVector.Split(',');
		return new Vector3(
			float.Parse(startPosStrings[0]),
			float.Parse(startPosStrings[1]),
			float.Parse(startPosStrings[2])
		);
		
	}

	public void leave() {
		GameObject menuController = transform.parent.gameObject;
		MenuController menuControllerScript = menuController.GetComponent<MenuController>();
		menuControllerScript.appearForm(MenuController.Forms.MAINMENU);
	}
}
