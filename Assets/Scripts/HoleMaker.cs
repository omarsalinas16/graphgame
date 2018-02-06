using UnityEngine;
using Parabox.CSG;

[System.Serializable]
public class HoleMaker {
	[SerializeField]
	private float axisPositionMin = -3.0f;
	[SerializeField]
	private float axisPositionMax = 3.0f;
	[SerializeField]
	private float axisStep = 0.3f;

	public void makeHole(Transform plane, GameObject form, Vector3 moveAxis) {
		GameObject planeGO = plane.gameObject;
		Vector3 originalPos = plane.position;
		float axisPosition = axisPositionMax;

		Mesh mesh = null;

		while (axisPosition > axisPositionMin) {
			plane.position = moveAxis * axisPosition;
			mesh = CSG.Subtract(planeGO, form);

			if (mesh != null) {
				mesh.name = "Generated Mesh";
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();

				MeshFilter meshFilter = planeGO.GetComponent<MeshFilter>();

				if (meshFilter != null) {
					GameObject.DestroyImmediate(planeGO.GetComponent<MeshFilter>());
					MeshFilter filter = planeGO.AddComponent<MeshFilter>();

					filter.sharedMesh = mesh;
				}

				MeshCollider planeCollider = planeGO.GetComponent<MeshCollider>();

				if (planeCollider != null) {
					GameObject.DestroyImmediate(planeGO.GetComponent<MeshCollider>());
					MeshCollider collider = planeGO.AddComponent<MeshCollider>();

					collider.sharedMesh = mesh;
				}
			}

			plane.position = originalPos;
			plane.localScale = Vector3.one;

			axisPosition -= axisStep;
		}
	}
}
