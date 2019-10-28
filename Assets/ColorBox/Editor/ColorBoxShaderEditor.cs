using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 
using System;
using Digicrafts.ColorBox;

namespace Digicrafts.Editor {

	/// <summary>
	/// Property action.
	/// </summary>
	public enum PropertyAction { None, Hide, DISABLE}

	/// <summary>
	/// Shader property info.
	/// </summary>
	public class ShaderPropertyInfo {

		public string name;
		public string[] keywords;
		public PropertyAction action;
		public int indentLevel;

		public ShaderPropertyInfo(string name, string[] keywords, int indentLevel =0, PropertyAction action = PropertyAction.None){			
			this.name = name;
			this.keywords = keywords;
			this.action = action;
			this.indentLevel = indentLevel;

		}
	}

	public class ShaderEditorStyles 
	{

		public static GUIStyle logo;
		public static GUIStyle logoTitle;
		public static Texture2D logoImage;

		public static GUIStyle sectionTitle;

		public static void initImages(string path){

			if(logoImage==null){
				if(EditorGUIUtility.isProSkin)
					logoImage=AssetDatabase.LoadAssetAtPath<Texture2D>(path+"logo_pro.png");
				else
					logoImage=AssetDatabase.LoadAssetAtPath<Texture2D>(path+"logo.png");
			}
		}

		public static void init(){

			if(logo==null){

				// Logo Title
				logoTitle = new GUIStyle(GUI.skin.GetStyle("Label"));
				logoTitle.fontSize=25;

				//logo
				logo = new GUIStyle(GUI.skin.GetStyle("Label"));
				logo.alignment = TextAnchor.UpperRight;
				logo.stretchWidth=false;
				logo.stretchHeight=false;
				logo.normal.background=logoImage;

				//sectionTitle
				sectionTitle =  new GUIStyle(GUI.skin.GetStyle("Label"));
				sectionTitle.fontStyle=FontStyle.Bold;
				sectionTitle.fontSize=14;
			}
		}
	}

	/// <summary>
	/// Gradient position slider drawer.
	/// </summary>
	public class GradientPositionSliderDrawer : MaterialPropertyDrawer 
	{

		public override void OnGUI (Rect position, MaterialProperty prop, String label, MaterialEditor editor)
		{

			//			Debug.Log(position);
			if(prop.type == MaterialProperty.PropType.Vector){

				Vector4 value = prop.vectorValue;
				float min = (float)Math.Round(value.x,2);
				float max = (float)Math.Round(value.y,2);
				float lableWidth = position.width/3;
				EditorGUI.BeginChangeCheck ();
				EditorGUI.LabelField(new Rect(position.x,position.y,lableWidth,position.height),label);
				EditorGUI.MinMaxSlider(new Rect(position.x+lableWidth,position.y,position.width-lableWidth,position.height),ref min, ref max, 0, 1);
				if (EditorGUI.EndChangeCheck ()) {
					value.x = min;
					value.y = max;
					prop.vectorValue = value;
				}
				//				EditorGUILayout.EndHorizontal();

			}

		}

	}

	/// <summary>
	/// Base shader editor.
	/// </summary>
	public class BaseShaderEditor : ShaderGUI
	{

		protected string title = "Color Box";

		virtual public Dictionary<string, ShaderPropertyInfo> GetShaderPropertyInfos() {
			return null;
		}

		public void DrawHeader()
		{
			ShaderEditorStyles.initImages("Assets/Digicrafts/ColorBox/Editor/");
			ShaderEditorStyles.init();

			/////////////////////////////////////////////////////////////////////////////////
			//// Logo
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();//IAPEditorStyles.logoBackground
			EditorGUILayout.LabelField(title,ShaderEditorStyles.logoTitle,GUILayout.Height(40));
			EditorGUILayout.LabelField("",ShaderEditorStyles.logo,GUILayout.Width(50),GUILayout.Height(40));
			EditorGUILayout.EndHorizontal();
			GUILayout.Box(GUIContent.none,GUILayout.ExpandWidth(true),GUILayout.Height(1));
			EditorGUILayout.Space();
			//// Logo
			/// /////////////////////////////////////////////////////////////////////////////////
		}

		protected void DrawHorizontalLine()
		{
			GUILayout.Box(GUIContent.none,GUILayout.ExpandWidth(true),GUILayout.Height(1));		
		}

		protected void DrawSectionHeader(string title)
		{
			EditorGUILayout.LabelField(title,ShaderEditorStyles.sectionTitle,GUILayout.Height(20));
			//			GUILayout.Box(GUIContent.none,GUILayout.ExpandWidth(true),GUILayout.Height(1));
			DrawHorizontalLine();
			EditorGUILayout.Space ();
		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		/// <param name="materialEditor">Material editor.</param>
		/// <param name="properties">Properties.</param>
		override public void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
		{

			DrawHeader();

			// get the current keywords from the material
			Material targetMat = materialEditor.target as Material;

			foreach(MaterialProperty p in properties){
				
				if(GetShaderPropertyInfos().ContainsKey(p.name)){

					ShaderPropertyInfo info = GetShaderPropertyInfos()[p.name];

					if(info.action == PropertyAction.None){

						materialEditor.ShaderProperty(p,p.displayName,info.indentLevel);

					} else {

						if(info.keywords!=null){
							//Loop for keywords
							foreach(string keyword in info.keywords){

								if(targetMat.IsKeywordEnabled(keyword)){

									materialEditor.ShaderProperty(p,p.displayName,info.indentLevel);

								} else {

									if(info.action == PropertyAction.Hide){

									} else if(info.action == PropertyAction.DISABLE){

										EditorGUI.BeginDisabledGroup(true);
										materialEditor.ShaderProperty(p,p.displayName,info.indentLevel);
										EditorGUI.EndDisabledGroup();
									}
								}
							}
						} else {

						}

					}

				} else {					
//					Debug.Log("pp " + p.displayName);
					materialEditor.ShaderProperty(p,p.displayName,0);
				}

			}				
		}
	} // BaseShaderEditor

	/// <summary>
	/// Color shader editor.
	/// </summary>
	[InitializeOnLoad]
	public class ColorBoxShaderEditor : BaseShaderEditor
	{		
		/// <summary>
		/// The shader property infos.
		/// </summary>
		public static Dictionary<string, ShaderPropertyInfo> _shaderPropertyInfos;

		override public Dictionary<string, ShaderPropertyInfo> GetShaderPropertyInfos() {
			return _shaderPropertyInfos;
		}

		/// <summary>
		/// Initializes the <see cref="Digicrafts.Editor.ColorShaderEditor"/> class.
		/// </summary>
		static ColorBoxShaderEditor(){
								
			_shaderPropertyInfos = new Dictionary<string, ShaderPropertyInfo>();

			_shaderPropertyInfos.Add("_MixColorPower",new ShaderPropertyInfo("_MixColorPower",new string[]{"_MIXCOLOR_ON"},1,PropertyAction.DISABLE));

			_shaderPropertyInfos.Add("_TopColor",new ShaderPropertyInfo("_TopColor",new string[]{"_TOP_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_TopColor1",new ShaderPropertyInfo("_TopColor1",new string[]{"_TOP_COLOR_GRADIENT","_TOP_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_TopColor2",new ShaderPropertyInfo("_TopColor2",new string[]{"_TOP_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_TopColor3",new ShaderPropertyInfo("_TopColor3",new string[]{"_TOP_COLOR_GRADIENT","_TOP_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_TopColorRotation",new ShaderPropertyInfo("_TopColorRotation",new string[]{"_TOP_COLOR_GRADIENT","_TOP_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_TopColorPosition",new ShaderPropertyInfo("_TopColorPosition",new string[]{"_TOP_COLOR_GRADIENT","_TOP_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_TopColorPosition3",new ShaderPropertyInfo("_TopColorPosition3",new string[]{"_TOP_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
					

			_shaderPropertyInfos.Add("_BottomColor",new ShaderPropertyInfo("_BottomColor",new string[]{"_BOTTOM_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BottomColor1",new ShaderPropertyInfo("_BottomColor1",new string[]{"_BOTTOM_COLOR_GRADIENT","_BOTTOM_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BottomColor2",new ShaderPropertyInfo("_BottomColor2",new string[]{"_BOTTOM_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BottomColor3",new ShaderPropertyInfo("_BottomColor3",new string[]{"_BOTTOM_COLOR_GRADIENT","_BOTTOM_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BottomColorRotation",new ShaderPropertyInfo("_BottomColorRotation",new string[]{"_BOTTOM_COLOR_GRADIENT","_BOTTOM_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BottomColorPosition",new ShaderPropertyInfo("_BottomColorPosition",new string[]{"_BOTTOM_COLOR_GRADIENT","_BOTTOM_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));		
			_shaderPropertyInfos.Add("_BottomColorPosition3",new ShaderPropertyInfo("_BottomColorPosition3",new string[]{"_BOTTOM_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));		


			_shaderPropertyInfos.Add("_FrontColor",new ShaderPropertyInfo("_FrontColor",new string[]{"_FRONT_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_FrontColor1",new ShaderPropertyInfo("_FrontColor1",new string[]{"_FRONT_COLOR_GRADIENT","_FRONT_COLOR_GRADIENT3"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_FrontColor2",new ShaderPropertyInfo("_FrontColor2",new string[]{"_FRONT_COLOR_GRADIENT3"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_FrontColor3",new ShaderPropertyInfo("_FrontColor3",new string[]{"_FRONT_COLOR_GRADIENT","_FRONT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_FrontColorRotation",new ShaderPropertyInfo("_FrontColorRotation",new string[]{"_FRONT_COLOR_GRADIENT","_FRONT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_FrontColorPosition",new ShaderPropertyInfo("_FrontColorPosition",new string[]{"_FRONT_COLOR_GRADIENT","_FRONT_COLOR_GRADIENT3"},1,PropertyAction.Hide));				
			_shaderPropertyInfos.Add("_FrontColorPosition3",new ShaderPropertyInfo("_FrontColorPosition3",new string[]{"_FRONT_COLOR_GRADIENT3"},1,PropertyAction.Hide));				

			_shaderPropertyInfos.Add("_BackColor",new ShaderPropertyInfo("_BackColor",new string[]{"_BACK_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BackColor1",new ShaderPropertyInfo("_BackColor1",new string[]{"_BACK_COLOR_GRADIENT","_BACK_COLOR_GRADIENT3"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BackColor2",new ShaderPropertyInfo("_BackColor2",new string[]{"_BACK_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BackColor3",new ShaderPropertyInfo("_BackColor3",new string[]{"_BACK_COLOR_GRADIENT","_BACK_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BackColorRotation",new ShaderPropertyInfo("_BackColorRotation",new string[]{"_BACK_COLOR_GRADIENT","_BACK_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BackColorPosition",new ShaderPropertyInfo("_BackColorPosition",new string[]{"_BACK_COLOR_GRADIENT","_BACK_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BackColorPosition3",new ShaderPropertyInfo("_BackColorPosition3",new string[]{"_BACK_COLOR_GRADIENT3"},1,PropertyAction.Hide));					

			_shaderPropertyInfos.Add("_LeftColor",new ShaderPropertyInfo("_LeftColor",new string[]{"_LEFT_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_LeftColor1",new ShaderPropertyInfo("_LeftColor1",new string[]{"_LEFT_COLOR_GRADIENT","_LEFT_COLOR_GRADIENT3"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_LeftColor2",new ShaderPropertyInfo("_LeftColor2",new string[]{"_LEFT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_LeftColor3",new ShaderPropertyInfo("_LeftColor3",new string[]{"_LEFT_COLOR_GRADIENT","_LEFT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_LeftColorRotation",new ShaderPropertyInfo("_LeftColorRotation",new string[]{"_LEFT_COLOR_GRADIENT","_LEFT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_LeftColorPosition",new ShaderPropertyInfo("_LeftColorPosition",new string[]{"_LEFT_COLOR_GRADIENT","_LEFT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_LeftColorPosition3",new ShaderPropertyInfo("_LeftColorPosition3",new string[]{"_LEFT_COLOR_GRADIENT3"},1,PropertyAction.Hide));					

			_shaderPropertyInfos.Add("_RightColor",new ShaderPropertyInfo("_RightColor",new string[]{"_RIGHT_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_RightColor1",new ShaderPropertyInfo("_RightColor1",new string[]{"_RIGHT_COLOR_GRADIENT","_RIGHT_COLOR_GRADIENT3"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_RightColor2",new ShaderPropertyInfo("_RightColor2",new string[]{"_RIGHT_COLOR_GRADIENT3"},1,PropertyAction.Hide));	
			_shaderPropertyInfos.Add("_RightColor3",new ShaderPropertyInfo("_RightColor3",new string[]{"_RIGHT_COLOR_GRADIENT","_RIGHT_COLOR_GRADIENT3"},1,PropertyAction.Hide));	
			_shaderPropertyInfos.Add("_RightColorRotation",new ShaderPropertyInfo("_RightColorRotation",new string[]{"_RIGHT_COLOR_GRADIENT","_RIGHT_COLOR_GRADIENT3"},1,PropertyAction.Hide));	
			_shaderPropertyInfos.Add("_RightColorPosition",new ShaderPropertyInfo("_RightColorPosition",new string[]{"_RIGHT_COLOR_GRADIENT","_RIGHT_COLOR_GRADIENT3"},1,PropertyAction.Hide));	
			_shaderPropertyInfos.Add("_RightColorPosition3",new ShaderPropertyInfo("_RightColorPosition3",new string[]{"_RIGHT_COLOR_GRADIENT3"},1,PropertyAction.Hide));	

			_shaderPropertyInfos.Add("_MainTex",new ShaderPropertyInfo("_MainTex",new string[]{"_TEXTUREENABLED_ON"},1,PropertyAction.DISABLE));
			_shaderPropertyInfos.Add("_UVSec",new ShaderPropertyInfo("_UVSec",new string[]{"_TEXTUREENABLED_ON"},1,PropertyAction.DISABLE));

//			_shaderPropertyInfos.Add("_Shadow",new ShaderPropertyInfo("_Shadow","_SHADOW_ON",1,PropertyAction.None));
			_shaderPropertyInfos.Add("_ShadowColor",new ShaderPropertyInfo("_ShadowColor",new string[]{"_SHADOW_ON"},1,PropertyAction.DISABLE));
			_shaderPropertyInfos.Add("_ShadowPower",new ShaderPropertyInfo("_ShadowPower",new string[]{"_SHADOW_ON"},1,PropertyAction.DISABLE));


//			_shaderPropertyInfos.Add("_VertexColor",new ShaderPropertyInfo("_VertexColor",new string[]{"_VERTEX_COLOR_ON"},,PropertyAction.None));

			_shaderPropertyInfos.Add("_DiffuseColor",new ShaderPropertyInfo("_DiffuseColor",new string[]{"_DIFFUSE_ON"},1,PropertyAction.DISABLE));
			_shaderPropertyInfos.Add("_DiffusePower",new ShaderPropertyInfo("_DiffusePower",new string[]{"_DIFFUSE_ON"},1,PropertyAction.DISABLE));

			_shaderPropertyInfos.Add("_AmbientColor",new ShaderPropertyInfo("_AmbientColor",new string[]{"_AMBIENT_ON"},1,PropertyAction.DISABLE));
			_shaderPropertyInfos.Add("_AmbientPower",new ShaderPropertyInfo("_AmbientPower",new string[]{"_AMBIENT_ON"},1,PropertyAction.DISABLE));

			_shaderPropertyInfos.Add("_SpecColor",new ShaderPropertyInfo("_SpecColor",new string[]{"_SPECULAR_ON"},1,PropertyAction.DISABLE));
			_shaderPropertyInfos.Add("_Shininess",new ShaderPropertyInfo("_Shininess",new string[]{"_SPECULAR_ON"},1,PropertyAction.DISABLE));

			_shaderPropertyInfos.Add("_LightmapPower",new ShaderPropertyInfo("_LightmapPower",new string[]{"_LIGHTMAP_ON"},1,PropertyAction.DISABLE));

		}

		override public void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			AddHelper();
			base.OnGUI(materialEditor,properties);
		}

		/// <summary>
		/// Assigns the new shader to material.
		/// </summary>
		/// <param name="material">Material.</param>
		/// <param name="oldShader">Old shader.</param>
		/// <param name="newShader">New shader.</param>
		override public void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
		{
			AddHelper();
			base.AssignNewShaderToMaterial(material,oldShader,newShader);
		}

//		override public void OnMaterialPreviewGUI(MaterialEditor materialEditor, Rect r, GUIStyle background)
//		{ 			
//			base.OnMaterialPreviewGUI(materialEditor,r,background);
//		}	

		/// <summary>
		/// Adds the helper.
		/// </summary>
		private void AddHelper(){
			if(Selection.activeObject){
				foreach(GameObject obj in Selection.gameObjects){
					if(obj.GetComponent<ColorBoxShaderHelper>() == null) {
						// Add the helper class
						obj.AddComponent<ColorBoxShaderHelper>();
						// Update helper class

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
				}

			}
		}

	} // Class ColorGradientShaderEditor

	[InitializeOnLoad]
	public class ColorBoxSkyboxEditor : BaseShaderEditor
	{		
		/// <summary>
		/// The shader property infos.
		/// </summary>
		public static Dictionary<string, ShaderPropertyInfo> _shaderPropertyInfos;

		override public Dictionary<string, ShaderPropertyInfo> GetShaderPropertyInfos() {
			return _shaderPropertyInfos;
		}

		/// <summary>
		/// Initializes the <see cref="Digicrafts.Editor.ColorShaderEditor"/> class.
		/// </summary>
		static ColorBoxSkyboxEditor(){

			_shaderPropertyInfos = new Dictionary<string, ShaderPropertyInfo>();
			_shaderPropertyInfos.Add("_BgColor",new ShaderPropertyInfo("_BgColor",new string[]{"_BG_COLOR_SOLID"},1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BgColor1",new ShaderPropertyInfo("_BgColor1",new string[]{"_BG_COLOR_GRADIENT","_BG_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BgColor2",new ShaderPropertyInfo("_BgColor2",new string[]{"_BG_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));
			_shaderPropertyInfos.Add("_BgColor3",new ShaderPropertyInfo("_BgColor3",new string[]{"_BG_COLOR_GRADIENT","_BG_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BgColorRotation",new ShaderPropertyInfo("_BgColorRotation",new string[]{"_BG_COLOR_GRADIENT","_BG_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BgColorPosition",new ShaderPropertyInfo("_BgColorPosition",new string[]{"_BG_COLOR_GRADIENT","_BG_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					
			_shaderPropertyInfos.Add("_BgColorPosition3",new ShaderPropertyInfo("_BgColorPosition3",new string[]{"_BG_COLOR_GRADIENT3"} ,1,PropertyAction.Hide));					

		}
			

	} // Class ColorGradientShaderEditor



}