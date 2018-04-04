using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

namespace HoleMakerHelpers {
	public class ExtractFaces {

		GameObject gameObject;

		public enum FACE {
			X, Z
		}

		private FACE faceToExtract;

		public ExtractFaces(GameObject gameObject, FACE face) {
			this.gameObject = gameObject;
			if (face == FACE.X) modifyMeshCube();
			faceToExtract = face;
		}

		private void modifyMeshCube() {
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

			var collider = gameObject.GetComponent<MeshCollider>();
			var boxCollider = gameObject.GetComponent<BoxCollider>();
			GameObject.DestroyImmediate(boxCollider);

			if (!collider) {
				collider = gameObject.AddComponent<MeshCollider>();
				collider.sharedMesh = meshFilter.mesh;
				
				collider.convex = true;
				collider.isTrigger = true;
			}
		}

		private List<Int32> getTrianglesInFront() {
			Vector3 originalPos = gameObject.transform.position;
			Renderer rend = gameObject.GetComponent<Renderer>();
			Bounds bounds = rend.bounds;

			Vector3 center = bounds.center;
			Vector3 extents = rend.bounds.extents;
			Vector3 begin = center - extents;

			float jumpsFace = 0.1f;
			float jumpsY = 0.1f;

			float limiteFace; // new
			float beginFace;
			if (faceToExtract == FACE.X) {
				beginFace = begin.z;
				limiteFace = begin.z + (extents.z * 2);
			} else {
				beginFace = begin.x;
				limiteFace = begin.x + (extents.x * 2);
			}
			float limitY = begin.y + (extents.y * 2);
			// Tal vez esto es lo que cause que falle cuando hay una translacion
			// Tengo que probar si falla en la translacion
			float STEP = -2.0f; // new 

			List<Int32> trianglesDetected = new List<Int32>();

			//collider of cube
			var colliderCube = gameObject.GetComponent<MeshCollider>();
			/* for(float z = begin.z; z < limitZ;z+=jumpsZ ) {							
				for(float y = begin.y; y < limitY;y+=jumpsY) {							
					RaycastHit hitN;
					Vector3 point = new Vector3(XFIJO,y,z);				
					if(Physics.Raycast(point, Vector3.right, out hitN)) {
						int triangleIndex = hitN.triangleIndex;										
						if(!trianglesDetected.Exists(t => t == triangleIndex)) {
							if(hitN.collider == colliderCube) {
								trianglesDetected.Add(hitN.triangleIndex);							
							}						
						}				
					}
				}
            } */  // old



			for (float f = beginFace; f < limiteFace; f += jumpsFace) {
				for (float y = begin.y; y < limitY; y += jumpsY) {
					RaycastHit hitN;
					Vector3 point;
					bool rayCastOk;
					if (faceToExtract == FACE.X) {
						point = new Vector3(STEP, y, f);
						rayCastOk = Physics.Raycast(point, Vector3.right, out hitN);
					} else {
						point = new Vector3(f, y, -STEP);
						rayCastOk = Physics.Raycast(point, Vector3.back, out hitN);
					}
					if (rayCastOk) {
						int triangleIndex = hitN.triangleIndex;
						if (!trianglesDetected.Exists(t => t == triangleIndex)) {
							if (hitN.collider == colliderCube) {

								trianglesDetected.Add(hitN.triangleIndex);
							}
						}
					}
				}
			}

			return trianglesDetected;
		}

		private void getTrianglesAndVertices(
			// The inputs
			List<Int32> trianglesDetected,
			Mesh mesh,
			// The outputs
			out List<Int32> indicesVerticesDetected_f1,
			out int[] trianglesFront_f1,
			out Vector3[] verticesWithDetectedFrontFace_f1,
			out Vector3[] verticesWithDetectedBackFace_f1,
			out Vector2[] uv_f1,
			out Vector2[] verticesFrontGlobalCordinates) {


			Vector3[] vertices_f1 = mesh.vertices;
			verticesWithDetectedFrontFace_f1 = mesh.vertices;
			verticesWithDetectedBackFace_f1 = mesh.vertices;
			uv_f1 = mesh.uv;
			int[] triangles = mesh.triangles;

			// This will help to obtain the borders
			verticesFrontGlobalCordinates = new Vector2[mesh.vertices.Length];

			indicesVerticesDetected_f1 = new List<Int32>();


			trianglesDetected = trianglesDetected.OrderBy(i => i).ToList();
			trianglesFront_f1 = new int[trianglesDetected.Count * 3];
			int tIndex = 0;

			foreach (Int32 t in trianglesDetected) {

				for (int tPlus = 0; tPlus < 3; tPlus++) {
					int triangleIndex = t * 3 + tPlus;
					int indexVertice = triangles[triangleIndex];

					trianglesFront_f1[tIndex++] = indexVertice;
					if (indicesVerticesDetected_f1.Exists(v_ => v_ == indexVertice)) {
						// Esta repetido no lo necesito convertir ni agregar de nuevo
						continue;
					}

					indicesVerticesDetected_f1.Add(indexVertice);
					Vector3 v = vertices_f1[indexVertice];
					Vector3 vFront = new Vector3(v.x, v.y, v.z);
					Vector3 vBack = new Vector3(v.x, v.y, v.z);

					Vector3 vectorToTransformFront = gameObject.transform.TransformPoint(vFront);
					Vector3 vectorToTransformBack = gameObject.transform.TransformPoint(vBack);

					if (faceToExtract == FACE.X) {
						vectorToTransformFront.x = -3f;
						vectorToTransformBack.x = 3f;
						verticesFrontGlobalCordinates[indexVertice] = new Vector2(vectorToTransformFront.z, vectorToTransformFront.y);

					} else {
						vectorToTransformFront.z = 2.5f;
						vectorToTransformBack.z = -2.5f;
						verticesFrontGlobalCordinates[indexVertice] = new Vector2(vectorToTransformFront.x, vectorToTransformFront.y);
					}


					//Vector3 vectorTransformedFront = gameObject.transform.InverseTransformPoint(vectorToTransformFront);								
					verticesWithDetectedFrontFace_f1[indexVertice] = vectorToTransformFront;


					//Vector3 vectorTransformedBack = gameObject.transform.InverseTransformPoint(vectorToTransformBack);				
					verticesWithDetectedBackFace_f1[indexVertice] = vectorToTransformBack;
				}
			}

			indicesVerticesDetected_f1 = indicesVerticesDetected_f1.OrderBy(i => i).ToList();

		}

		public Mesh getFaceDeprecated(out int[] verticesInCenter) {
			var colliderCube = gameObject.GetComponent<MeshCollider>();

			List<Int32> trianglesDetected = getTrianglesInFront();

			List<Int32> indicesVerticesDetected_f1;
			int[] trianglesFront_f1;
			Vector3[] verticesWithDetectedFrontFace_f1,
						verticesWithDetectedBackFace_f1;
			Vector2[] uv_f1;
			Vector2[] verticesFrontGlobalCordinates;


			getTrianglesAndVertices(
				trianglesDetected,
				colliderCube.sharedMesh,
				out indicesVerticesDetected_f1,
				out trianglesFront_f1,
				out verticesWithDetectedFrontFace_f1,
				out verticesWithDetectedBackFace_f1,
				out uv_f1,
				out verticesFrontGlobalCordinates
			);

			int[] trianglesFrontFace_f2 = new int[trianglesDetected.Count * 3];
			int[] trianglesBackFace_f2 = new int[trianglesDetected.Count * 3];

			Vector3[] verticesFrontFace_f2 = new Vector3[indicesVerticesDetected_f1.Count];
			Vector3[] verticesBackFace_f2 = new Vector3[indicesVerticesDetected_f1.Count];
			Vector2[] uv_f2 = new Vector2[indicesVerticesDetected_f1.Count];

			int indexVertice_f2 = 0;

			foreach (Int32 indexVertice_f1 in indicesVerticesDetected_f1) {
				verticesFrontFace_f2[indexVertice_f2] = verticesWithDetectedFrontFace_f1[indexVertice_f1];
				verticesBackFace_f2[indexVertice_f2] = verticesWithDetectedBackFace_f1[indexVertice_f1];

				uv_f2[indexVertice_f2] = uv_f1[indexVertice_f1];
				for (int t = 0; t < trianglesFront_f1.Length; t++) {
					if (trianglesFront_f1[t] == indexVertice_f1) {
						trianglesFrontFace_f2[t] = indexVertice_f2;
						trianglesBackFace_f2[t] = indexVertice_f2 + verticesBackFace_f2.Length;
					}
				}
				indexVertice_f2++;
			}

			Vector3[] verticesForHole = new Vector3[verticesFrontFace_f2.Length + verticesBackFace_f2.Length];
			Array.Copy(verticesFrontFace_f2, verticesForHole, verticesFrontFace_f2.Length);
			Array.Copy(verticesBackFace_f2, 0, verticesForHole, verticesFrontFace_f2.Length, verticesBackFace_f2.Length);

			Vector2[] uvsForHole = new Vector2[uv_f2.Length + uv_f2.Length];
			Array.Copy(uv_f2, uvsForHole, uv_f2.Length);
			Array.Copy(uv_f2, 0, uvsForHole, uv_f2.Length, uv_f2.Length);


			int offset = verticesFrontFace_f2.Length;

			Mesh meshFront_f1 = new Mesh();
			meshFront_f1.vertices = verticesFrontFace_f2;
			meshFront_f1.triangles = trianglesFrontFace_f2;
			meshFront_f1.uv = uv_f2;

			verticesInCenter = VertexClassifier.getVerticesInCenter(meshFront_f1, verticesFrontGlobalCordinates, gameObject.transform);

			return meshFront_f1;
		}

		public Mesh getFace(out int[] verticesInCenter) {
			var colliderCube = gameObject.GetComponent<MeshCollider>();

			List<Int32> trianglesDetected = getTrianglesInFront();

			List<Int32> indicesVerticesDetected_f1;
			int[] trianglesFront_f1;
			Vector3[] verticesWithDetectedFrontFace_f1,
						verticesWithDetectedBackFace_f1;
			Vector2[] uv_f1;
			Vector2[] verticesFrontGlobalCordinates;


			getTrianglesAndVertices(
				trianglesDetected,
				colliderCube.sharedMesh,
				out indicesVerticesDetected_f1,
				out trianglesFront_f1,
				out verticesWithDetectedFrontFace_f1,
				out verticesWithDetectedBackFace_f1,
				out uv_f1,
				out verticesFrontGlobalCordinates
			);

			int[] trianglesFrontFace_f2 = new int[trianglesDetected.Count * 3];
			int[] trianglesBackFace_f2 = new int[trianglesDetected.Count * 3];

			Vector3[] verticesFrontFace_f2 = new Vector3[indicesVerticesDetected_f1.Count];
			Vector3[] verticesBackFace_f2 = new Vector3[indicesVerticesDetected_f1.Count];
			Vector2[] uv_f2 = new Vector2[indicesVerticesDetected_f1.Count];

			int indexVertice_f2 = 0;

			foreach (Int32 indexVertice_f1 in indicesVerticesDetected_f1) {
				verticesFrontFace_f2[indexVertice_f2] = verticesWithDetectedFrontFace_f1[indexVertice_f1];
				verticesBackFace_f2[indexVertice_f2] = verticesWithDetectedBackFace_f1[indexVertice_f1];

				uv_f2[indexVertice_f2] = uv_f1[indexVertice_f1];
				for (int t = 0; t < trianglesFront_f1.Length; t++) {
					if (trianglesFront_f1[t] == indexVertice_f1) {
						trianglesFrontFace_f2[t] = indexVertice_f2;
						trianglesBackFace_f2[t] = indexVertice_f2 + verticesBackFace_f2.Length;
					}
				}
				indexVertice_f2++;
			}

			int offset = verticesFrontFace_f2.Length;

			List<Vector3> listVerticesFrontFace_f2 = new List<Vector3>();
			List<Vector2> listUvFrontFace_f2 = new List<Vector2>();

			for (int vf = 0; vf < verticesFrontFace_f2.Length; vf++) {
				Vector3 currentVertex = verticesFrontFace_f2[vf];
				// Checar si se repite
				if (!listVerticesFrontFace_f2.Exists(notReptitiveVertex => notReptitiveVertex == currentVertex)) {
					// SI no se repite agregarlo
					listVerticesFrontFace_f2.Add(currentVertex);
					listUvFrontFace_f2.Add(uv_f2[vf]);
					int indexCurrentVertex = listVerticesFrontFace_f2.Count - 1;
					for (int t = 0; t < trianglesFrontFace_f2.Length; t++) {
						if (verticesFrontFace_f2[trianglesFrontFace_f2[t]] == currentVertex) {
							trianglesFrontFace_f2[t] = indexCurrentVertex;
						}
					}
				}
			}

			Mesh meshFront_f1 = new Mesh();
			meshFront_f1.vertices = listVerticesFrontFace_f2.ToArray();
			meshFront_f1.triangles = trianglesFrontFace_f2;
			meshFront_f1.uv = listUvFrontFace_f2.ToArray();

			verticesInCenter = VertexClassifier.getVerticesInCenter(meshFront_f1, verticesFrontGlobalCordinates, gameObject.transform);

			return meshFront_f1;
		}

	}

}