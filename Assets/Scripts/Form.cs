using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Form {
	[SerializeField]
	private string name;

	[SerializeField]
	private Transform _shape = null;
	public Transform shape {
		get {
			return _shape;
		}

		set {
			_shape = value;
		}
	}
	
	[Header("Solution")]
	[SerializeField]
	private Vector3 _position = new Vector3();
	public Vector3 position {
		get {
			return _position;
		}

		set {
			_position = value;
		}
	}
	[SerializeField]
	private Vector3 _rotation = new Vector3();
	public Vector3 rotation {
		get {
			return _rotation;
		}

		set {
			_rotation = value;
		}
	}
	[SerializeField]
	private Vector3 _scale = new Vector3();
	public Vector3 scale {
		get {
			return _scale;
		}

		set {
			_scale = value;
		}
	}
}
