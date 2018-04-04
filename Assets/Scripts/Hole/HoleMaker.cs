using HoleMakerHelpers;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using ConstructiveSolidGeometry;

namespace HoleMakerL {

    public class HoleMaker {
        GameObject form;
        GameObject xDeepObject;
        GameObject zDeepObject;

        public HoleMaker(GameObject form) {
            this.form = form;
            this.xDeepObject = getXObject();
            this.zDeepObject = getZObject();            
        }

        private GameObject getXObject() {
            ExtractFaces extractXFace = new ExtractFaces(this.form, ExtractFaces.FACE.X);            
			int[] verticesInCenter;
			Mesh planeMesh = extractXFace.getFace(out verticesInCenter);            

            GameObject go = new GameObject("FormForXPlane");			
			MeshFilter meshFilter = go.AddComponent<MeshFilter>();
			MeshCollider meshCollider = go.AddComponent<MeshCollider>();
			go.AddComponent<MeshRenderer>();		
			
			meshFilter.sharedMesh = planeMesh;
			meshCollider.sharedMesh = planeMesh;
            
			MeshExtrusion.Edge[] falseEdges = MeshExtrusion.BuildManifoldEdges(planeMesh);
			MeshExtrusion.Edge[] realEdges = VertexClassifier.getRealEdges(falseEdges, verticesInCenter);

            // Defining transformations to extrude the mesh.
			Matrix4x4[] sections = new Matrix4x4[2];

			// Starting point (where the mesh already is, but its needed so the new mesh does not generate with a hole).
			sections[0] = go.transform.localToWorldMatrix;
			// The end point.
			sections[1] = go.transform.worldToLocalMatrix * Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), new Vector3(1, 1, 1));

			// Get the mesh again (the script need a different reference to the same mesh for some reason?).
			Mesh deepMesh = new Mesh();
			// Actually perform the extrusion, the parameter [invertFaces] is extremely important in this case but its kind of hard to programatically tell when it is needed.
			// I would guess you would need to somehow see the direction of the normals before even creating the face.						
			MeshExtrusion.ExtrudeMesh(planeMesh, deepMesh, sections, realEdges, false);

            asignNewMesh(deepMesh,go);

            return go;
        }

        private GameObject getZObject() {
            ExtractFaces extractZFace = new ExtractFaces(this.form, ExtractFaces.FACE.Z);            
			int[] verticesInCenter;
			Mesh planeMesh = extractZFace.getFace(out verticesInCenter);                        

            Debug.Log("125 : Vertices" + planeMesh.vertices.Length);
            Debug.Log("125 : Triangles" + planeMesh.triangles.Length);

            GameObject go = new GameObject("FormForZPlane");			            
			MeshFilter meshFilter = go.AddComponent<MeshFilter>();
			MeshCollider meshCollider = go.AddComponent<MeshCollider>();
			go.AddComponent<MeshRenderer>();		
			
			meshFilter.sharedMesh = planeMesh;
			meshCollider.sharedMesh = planeMesh;
            
			MeshExtrusion.Edge[] falseEdges = MeshExtrusion.BuildManifoldEdges(planeMesh);
			MeshExtrusion.Edge[] realEdges = VertexClassifier.getRealEdges(falseEdges, verticesInCenter);

            // Defining transformations to extrude the mesh.
			Matrix4x4[] sections = new Matrix4x4[2];

			// Starting point (where the mesh already is, but its needed so the new mesh does not generate with a hole).
			sections[0] = go.transform.localToWorldMatrix;
			// The end point.
			sections[1] = go.transform.worldToLocalMatrix * Matrix4x4.TRS(new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0), new Vector3(1, 1, 1));

			// Get the mesh again (the script need a different reference to the same mesh for some reason?).
			Mesh deepMesh = new Mesh();
			// Actually perform the extrusion, the parameter [invertFaces] is extremely important in this case but its kind of hard to programatically tell when it is needed.
			// I would guess you would need to somehow see the direction of the normals before even creating the face.						
			MeshExtrusion.ExtrudeMesh(planeMesh, deepMesh, sections, realEdges, true);

            asignNewMesh(deepMesh,go);

            return go;
        }

        private void asignNewMesh(Mesh mesh, GameObject planeGO) {                        
            if (mesh != null) {
                mesh.name = "Deep Mesh";
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();

                MeshFilter meshFilter = planeGO.GetComponent<MeshFilter>();                
                if (meshFilter != null) {
                    UnityEngine.Object.DestroyImmediate(planeGO.GetComponent<MeshFilter>());
                    MeshFilter filter = planeGO.AddComponent<MeshFilter>();

                    filter.sharedMesh = mesh;
                }

                MeshCollider planeCollider = planeGO.GetComponent<MeshCollider>();

                if (planeCollider != null) {
                    UnityEngine.Object.DestroyImmediate(planeGO.GetComponent<MeshCollider>());
                    MeshCollider collider = planeGO.AddComponent<MeshCollider>();
                    collider.sharedMesh = mesh;
                }
            }
        }

        public void makeXHole(GameObject xPlane) {
            makeHole(xPlane, xDeepObject);
            UnityEngine.Object.Destroy(xDeepObject);
        }

        public void makeZHole(GameObject zPlane) {            
            makeHole(zPlane, zDeepObject);
            UnityEngine.Object.Destroy(zDeepObject);
        }

        private void makeHole(GameObject plane, GameObject go) {            
            // Preparing CSG objects for substraction.
			// Again, passing a new refference to the same mesh is important for some reason.					 
			ConstructiveSolidGeometry.CSG planeToCut = ConstructiveSolidGeometry.CSG.fromMesh(plane.GetComponent<MeshFilter>().mesh, plane.transform);
			ConstructiveSolidGeometry.CSG formCutter = ConstructiveSolidGeometry.CSG.fromMesh(go.GetComponent<MeshFilter>().mesh, go.transform);

			// Save the operation, this does not affect neither meshes yet.
			ConstructiveSolidGeometry.CSG result = planeToCut.subtract(formCutter);

			// Replace the mesh of THIS object with the mesh representation of the operation.
			plane.GetComponent<MeshFilter>().mesh = result.toMesh();

            plane.transform.position = new Vector3(0,0,0);
		    plane.transform.localScale = Vector3.one;		 
        }


    }

}