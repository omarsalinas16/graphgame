using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace HoleMakerHelpers
{
	public class VertexClassifier {

		// This will obtain the index of the vertex in the border 
		public static int[] getBorders (Mesh mesh,Vector2[] verticesGlobalCord,Transform transform) {
			int[] triangles = mesh.triangles;
			Vector3[] vertices = mesh.vertices;
			List<Vector3> lstVert = vertices.OfType<Vector3>().ToList();
			List<Int32> vertexBoundsIndex = new List<Int32>();  
			for(int i = 0; i < lstVert.Count; i++) {
				for(int j = 0; j < lstVert.Count; j++) {
					if(i != j) {
						if(lstVert[i] == lstVert[j])
							lstVert.RemoveAt(j);
					}
				}
				//lstVert[i].x = 0.0f;
			}

			/*for(int i = 0; i < lstVert.Count; i++) {
				Debug.Log("List ver " + i + " : "+ lstVert[i]);
			}*/

			Vector3 centralVertex = new Vector3(0,0,0);
			for(int i = 0; i < lstVert.Count; i++) { // Iteracion en lista de vertices sin repetir
				float totalAngle = 0;
				for(int j = 0; j < triangles.Length; j++) {
					if(vertices[triangles[j]] == lstVert[i]) {
						// En el indice 0 voy a poner a j y en los demas a los otros
						Vector3[] vectorsFormTriangle = new Vector3[3];
						int p = j;
						if(p < 3) {
							p  += 3;
						}
						if(p % 3 == 0) {
							vectorsFormTriangle[0] = vertices[triangles[j]];
							vectorsFormTriangle[1] = vertices[triangles[j + 1]];
							vectorsFormTriangle[2] = vertices[triangles[j + 2]];
						} else if (p % 3 == 1) {
							vectorsFormTriangle[0] = vertices[triangles[j]];
							vectorsFormTriangle[1] = vertices[triangles[j - 1]];
							vectorsFormTriangle[2] = vertices[triangles[j + 1]];
						} else if (p % 3 == 2) {
							vectorsFormTriangle[0] = vertices[triangles[j]];
							vectorsFormTriangle[1] = vertices[triangles[j - 1]];
							vectorsFormTriangle[2] = vertices[triangles[j - 2]];
						}
						// cos(c) = (a^2 + b^2 - c^2) / 2ab
						float a = Vector3.Distance(vectorsFormTriangle[0],vectorsFormTriangle[1]);
						float b = Vector3.Distance(vectorsFormTriangle[0],vectorsFormTriangle[2]);
					 	float c = Vector3.Distance(vectorsFormTriangle[1],vectorsFormTriangle[2]);
						float angleC = Mathf.Acos( (Mathf.Pow(a,2) + Mathf.Pow(b,2) - Mathf.Pow(c,2)) / (2 * a * b) );
						angleC =(float) ( angleC * (180.0 / Mathf.PI) ); // conversion de radianes a grados
						totalAngle += angleC;
					}
				}	
				//Debug.Log("Total angle = " + totalAngle);		
				if(Math.Ceiling(totalAngle) >= 360) {
					//Debug.Log("Se borro: " + lstVert[i]);		
					//centralVertex = lstVert[i];
					lstVert.RemoveAt(i);
					//Debug.Log("Esta removiendo a: " + vertices[i]);
					
				}
			}

			int[] borders = new int[lstVert.Count];
			
			for(int i = 0; i < lstVert.Count; i++) {				
				for(int j = 0; j < vertices.Length; j++) {
					if(vertices[j] == lstVert[i]) {
						borders[i] = j;
						break;
					}
				}
			}
		
			return borders;
		}

        // This will obtain the index of the vertex in the border 
		public static int[] getVerticesInCenter (Mesh mesh,Vector2[] verticesGlobalCord,Transform transform) {
			

			int[] borders = getBorders(mesh, verticesGlobalCord, transform);			

			List<Int32> listVerticesInCenter = new List<Int32>();
            Vector3[] vertices = mesh.vertices;
			for(int v = 0; v < vertices.Length; v++) {
				bool isIn = false;
				for(int i = 0; i < borders.Length; i++) {				
					if(vertices[v] == vertices[borders[i]]) {
						isIn = true;
						break;
					}
				}
				if(!isIn) {					
					listVerticesInCenter.Add(v);
				}
			}			

			return listVerticesInCenter.ToArray();
		}


        // This will obtain the borders accomodated to the rigth
        public static int[] getBordersInOrder(Mesh mesh,Vector2[] verticesGlobalCord,Transform transform) {
            int[] borders = getBorders(mesh, verticesGlobalCord, transform);			
            //Acomodar los vertices a la derecha
			int[] bordersAcomodados = acomodarBordes(mesh.vertices,borders,new Vector3(0,0,0),transform);

            return bordersAcomodados;
        }		

        public static MeshExtrusion.Edge[] getRealEdges(MeshExtrusion.Edge[] falseEdges, int[] verticesInCenter) {
            List<MeshExtrusion.Edge> realEdges = new List<MeshExtrusion.Edge>();
			for(int e = 0; e < falseEdges.Length; e++) {
				bool isNotAnEdge = false;
				for(int vc = 0; vc < verticesInCenter.Length; vc++) {
					if(falseEdges[e].vertexIndex[0] == verticesInCenter[vc]
						||
						falseEdges[e].vertexIndex[1] == verticesInCenter[vc]
					) {
						isNotAnEdge = true;
						break;
					}
				}
				if(!isNotAnEdge) {
					realEdges.Add(falseEdges[e]);
				}
			}
            return realEdges.ToArray();
        }

		private static float GetAngle(Vector2 v1, Vector2 v2) {
			var sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
			return Vector2.Angle(v1, v2) * sign;
		}

		private static int[] acomodarBordes (Vector3[] vertices, int[] borders, Vector3 vectorInside,Transform transform) {
			List<VectorZsizeAndIndex> vectorUp = new List<VectorZsizeAndIndex>();
			List<VectorZsizeAndIndex> vectorDown = new List<VectorZsizeAndIndex>();
			int i;
			for(i = 0; i < borders.Length; i++) {
				Vector3 v = vertices[borders[i]];
				v = transform.TransformPoint(v);
				if(v.y >= 0) {
					vectorUp.Add(new VectorZsizeAndIndex(v.z,borders[i]));
					//Debug.Log("VUp: " + v);
				} else if (v.y < 0) {
					vectorDown.Add(new VectorZsizeAndIndex(v.z,borders[i]));
					//Debug.Log("VDown: " + v);
				}	
			}
			
			QuickSort(vectorUp, 0, vectorUp.Count - 1);
			
			QuickSort(vectorDown, 0, vectorDown.Count - 1);					

			int[] bordersUp = new int[vectorUp.Count];
			for(i = 0; i < vectorUp.Count; i++) {
				//Debug.Log("Vup:" + vertices[vectorUp[i].borderIndex]);
				bordersUp[i] = vectorUp[i].borderIndex;
			}
			int[] bordersDown = new int[vectorDown.Count];
			int j = vectorDown.Count;
			for(i = 0; i < vectorDown.Count; i++) {
				//Debug.Log("Vdown:"+vertices[vectorDown[i].borderIndex]);
				bordersDown[--j] = vectorDown[i].borderIndex;
			}
			int[] bordersAcomodados = new int[vectorUp.Count + vectorDown.Count];

			Array.Copy(bordersUp, bordersAcomodados, vectorUp.Count);
			Array.Copy(bordersDown, 0, bordersAcomodados, vectorUp.Count, vectorDown.Count);
			
			for(i = 0;i < bordersAcomodados.Length; i++) {
				Vector3 v2 = vertices[bordersAcomodados[i]];
				v2 = transform.TransformPoint(v2);
				//Debug.Log(v2);
			}

			return bordersAcomodados;
		}

		public static void QuickSort(List<VectorZsizeAndIndex> a, int start, int end)
        {
            if (start >= end)
            {
                return;
            }

            float num = a[start].z;

            int i = start, j = end;

            while (i < j)
            {
                while (i < j && a[j].z > num)
                {
                    j--;
                }

                a[i].z = a[j].z;

                while (i < j && a[i].z < num)
                {
                    i++;
                }

                a[j].z = a[i].z;
            }

            a[i].z = num;
            QuickSort(a, start, i - 1);
            QuickSort(a, i + 1, end);
        }
	}

	public class VectorZsizeAndIndex {
		public float z;
		public int borderIndex;
		//boolean yUpper; // y positiva = true : y negativa = false

		public VectorZsizeAndIndex (float z, int b) {
			this.z = z;
			this.borderIndex = b;
		}

		public String getString() {
			return "z: " + z + " bIndex: " + borderIndex;
		}
	}
}
