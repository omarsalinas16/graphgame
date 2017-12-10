using UnityEngine;

public class AxisXYZ : MonoBehaviour {
	private static Material lineMaterial = null;
	public static Color gridLinesColor = Color.green;

	[SerializeField]
	private float spaceBetweenLines = 1.0f;
	[SerializeField]
	private float gridWidth = 6.0f;
	[SerializeField]
	private float gridHeight = 8.0f;

	private static Material createLineMaterial() {
		Shader shader = Shader.Find("Hidden/Internal-Colored");
		Material material = new Material(shader);

		material.hideFlags = HideFlags.HideAndDontSave;
		// Turn on alpha blending
		material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		// Turn backface culling off
		material.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
		// Turn off depth writes
		material.SetInt("_ZWrite", 0);

		return material;
	}

	// Will be called after all regular rendering is done
	public void OnRenderObject() {
		if (!lineMaterial) {
			lineMaterial = createLineMaterial();
		} else {
			lineMaterial.SetPass(0);
			
			float halfWidth = gridWidth / 2;
			float halfHeight = gridHeight / 2;

			GL.PushMatrix();
			GL.MultMatrix(transform.localToWorldMatrix);

			// Draw lines
			GL.Begin(GL.LINES);
			GL.Color(gridLinesColor);

			for (float i = -halfWidth; i <= halfWidth; i += spaceBetweenLines) {
				GL.Vertex3(i, -halfHeight, 0);
				GL.Vertex3(i, halfHeight, 0);
			}

			GL.End();
			GL.PopMatrix();
		}
	}
}