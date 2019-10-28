using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Digicrafts.ColorBox {

	#if UNITY_EDITOR
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ColorBoxShaderHelper))]
	public class ColorBoxShaderHelperEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if(GUILayout.Button("Update"))
			{
				foreach(GameObject obj in Selection.gameObjects){
					if(obj!=null && Application.isEditor && !Application.isPlaying){						
//						Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
//						ColorBoxShaderHelper.updateMeshUV(mesh);
						ColorBoxShaderHelper.updateGameObjectMesh(obj);
					}
				}
			}
		}
	}
	#endif

	/// <summary>
	/// Color gradient shader helper.
	/// </summary>
	public class ColorBoxShaderHelper : MonoBehaviour {	

		private bool _updated = false;

		/// <summary>
		/// Raises the validate event.
		/// </summary>
		void OnValidate(){
			if(!_updated && Application.isEditor && !Application.isPlaying){

				updateGameObjectMesh(gameObject);
				_updated=true;
			}
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake () {
			updateGameObjectMesh(gameObject);
		}


		public static void updateGameObjectMesh(GameObject obj)
		{

			if(obj.GetComponent<MeshFilter>()!=null){
				Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
				if(mesh != null)
					ColorBoxShaderHelper.updateMeshUV(mesh);
			} else if(obj.GetComponent<SkinnedMeshRenderer>() != null){
				SkinnedMeshRenderer skin= obj.GetComponent<SkinnedMeshRenderer>();
				if(skin.sharedMesh)
					ColorBoxShaderHelper.updateMeshUV(skin.sharedMesh);
			}
		}

		/// <summary>
		/// Updates the mesh U.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		public static void updateMeshUV(Mesh mesh)
		{
			if(mesh!=null){

				Vector3[] vertices = mesh.vertices;
//				Color[] tangents = new Color[vertices.Length];
//				Vector2[] uv3 = new Vector2[vertices.Length];
//				Vector2[] uv4 = new Vector2[vertices.Length];
				Vector4[] tangents = new Vector4[vertices.Length];

				// Loop each submesh
				for (int k=0; k < mesh.subMeshCount; k++) {
					
					int[] index = mesh.GetTriangles(k);
					if(index.Length>0){
						Vector3 ext = Vector3.zero;
						Vector3 max = Vector3.zero;
						Vector3 min = Vector3.zero;

						// Calculate the bounds and center
						for (int i=0; i < index.Length; i++) {
							Vector3 v = vertices[index[i]];
							if(i==0){
								min = max = v;
							} else {
								max.x = Math.Max(v.x,max.x);
								max.y = Math.Max(v.y,max.y);
								max.z = Math.Max(v.z,max.z);
								min.x = Math.Min(v.x,min.x);
								min.y = Math.Min(v.y,min.y);
								min.z = Math.Min(v.z,min.z);
							}
						}
						ext.x=(max.x-min.x);
						ext.y=(max.y-min.y);
						ext.z=(max.z-min.z);

						// Calculate the uv
						for (int i=0; i < index.Length; i++) {
							Vector3 vertex = vertices[index[i]];					
							float u = ((vertex.x-min.x)/ext.x - 0.5f)*2;
							float v = ((vertex.y-min.y)/ext.y - 0.5f)*2;
							float w = ((vertex.z-min.z)/ext.z - 0.5f)*2;
//							tangents[index[i]] = new Color(u,v,w);
							tangents[index[i]] = new Vector4(u,v,w);
						}						
					}
				}
//				mesh.tangents
//				mesh.uv4=tangents;
				mesh.tangents=tangents;

			}
		}

	}

}