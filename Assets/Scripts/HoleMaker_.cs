using UnityEngine;
using ConstructiveSolidGeometry;

[System.Serializable]
public class HoleMaker_ {

	[SerializeField]
	private float extrudeLength = 10f;

	public void makeHoleWithCopy(GameObject plane, GameObject form, Vector3 moveAxis, bool invert = true, bool deleteAfter = true) {
		GameObject copy = Object.Instantiate(form);
		makeHole(plane, copy, moveAxis, invert);

		if (deleteAfter) {
			Object.Destroy(copy);
		}
	}

	public void makeHole(GameObject plane, GameObject form, Vector3 moveAxis, bool invert = false) {
		Mesh formMesh = getMesh(form);
		Mesh planeMesh = getMesh(plane);
		MeshExtrusion.Edge[] precomputedEdged = getEdges(formMesh);

		Matrix4x4[] extrusion = getExtrusion(form.transform, moveAxis * extrudeLength);
		
		MeshExtrusion.ExtrudeMesh(formMesh, formMesh, extrusion, precomputedEdged, invert);

		CSG _plane = CSG.fromMesh(planeMesh, plane.transform);
		CSG _form = CSG.fromMesh(formMesh, form.transform);
		
		CSG result = _plane.subtract(_form);
		Mesh resultingMesh = result.toMesh();

		plane.GetComponent<MeshFilter>().mesh = resultingMesh;
		plane.GetComponent<MeshCollider>().sharedMesh = resultingMesh;

		plane.transform.localScale = Vector3.one;
	}

	private Mesh getMesh(GameObject obj) {
		return obj.GetComponent<MeshFilter>().mesh;
	}

	private MeshExtrusion.Edge[] getEdges(Mesh mesh) {
		return MeshExtrusion.BuildManifoldEdges(mesh);
	}

	private Matrix4x4[] getExtrusion(Transform target, Vector3 translation) {
		Matrix4x4[] sections = new Matrix4x4[2];

		sections[0] = target.localToWorldMatrix;
		sections[1] = target.worldToLocalMatrix * Matrix4x4.TRS(translation, target.rotation, Vector3.one);

		return sections;
	}
}
