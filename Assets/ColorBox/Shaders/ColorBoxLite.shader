Shader "SG/Unlit/Color/ColorBoxLite"
{
	Properties
	{

		[Header(Colors)]			

		[Space(10)]
		_DefaultColor ("Main Color",Color) = (0.5,0.5,0.5,1)						
		[KeywordEnum(Object, World, Screen, Camera)] _UV_SPACE ("Gradient Space", Float) = 1
		[Toggle] _MixColor ("Mix Colors", Float) = 1
		// [Toggle(_SMOOTH_GRADIENT_ON)] _SmoothGradient ("Smooth Gradient", Float) = 1
		_MixColorPower ("Mix Power", Range(0,1)) = 0.5

		[Space(10)]

		[KeywordEnum(None,Solid,Gradient,Gradient3)] _TOP_COLOR ("Top Color (+y)", Float) = 1
		_TopColor (" ",Color) = (0.957, 1, 0.686,1)
		_TopColor1 ("Start Color",Color) = (0.667,0.851,0.937,1)
		_TopColor2 ("Middle Color",Color) = (0.29, 0.8, 0.2,1)
		_TopColor3 ("End Color",Color) = (0.29, 0.8, 0.2,1)
		[GradientPositionSliderDrawer]
		_TopColorPosition ("Gradient Position",Vector) = (0,1,0)
		_TopColorPosition3 ("Middle Size",Range(0,1)) = 0
		_TopColorRotation ("Gradient Rotation",Range(0,2)) = 0

		[Space(10)]

		[KeywordEnum(None,Solid,Gradient,Gradient3)] _BOTTOM_COLOR ("Bottom Color (-y)", Float) = 0
		_BottomColor (" ",Color) = (0, 0.835, 1,1)
		_BottomColor1 ("Start Color",Color) = (0.667,0.851,0.937,1)
		_BottomColor2 ("Middle Color",Color) = (0.29, 0.8, 0.2,1)
		_BottomColor3 ("End Color",Color) = (0.29, 0.8, 0.2,1)
		[GradientPositionSliderDrawer]
		_BottomColorPosition ("Gradient Position",Vector) = (0,1,0)
		_BottomColorPosition3 ("Middle Size",Range(0,1)) = 0
		_BottomColorRotation ("Gradient Rotation",Range(0,2)) = 0

		[Space(10)]

		[KeywordEnum(None,Solid,Gradient,Gradient3)] _FRONT_COLOR ("Front Color (-z)", Float) = 1
		_FrontColor (" ",Color) = (0.667,0.851,0.937,1)
		_FrontColor1 ("Start Color",Color) = (0.667,0.851,0.937,1)
		_FrontColor2 ("Middle Color",Color) = (0.29, 0.8, 0.2,1)
		_FrontColor3 ("End Color",Color) = (0.29, 0.8, 0.2,1)
		[GradientPositionSliderDrawer]
		_FrontColorPosition ("Gradient Position",Vector) = (0,1,0)
		_FrontColorPosition3 ("Middle Size",Range(0,1)) = 0
		_FrontColorRotation ("Gradient Rotation",Range(0,2)) = 0

		[Space(10)]

		[KeywordEnum(None,Solid,Gradient,Gradient3)] _BACK_COLOR ("Back Color (+z)", Float) = 0
		_BackColor (" ",Color) = (0.082, 1, 0.514,1)
		_BackColor1 ("Start Color",Color) = (0.082, 1, 0.514,1)
		_BackColor2 ("Middle Color",Color) = (1, 0.082, 0.624,1)
		_BackColor3 ("End Color",Color) = (1, 0.082, 0.624,1)
		[GradientPositionSliderDrawer]
		_BackColorPosition ("Gradient Position",Vector) = (0,1,0)
		_BackColorPosition3 ("Middle Size",Range(0,1)) = 0
		_BackColorRotation ("Gradient Rotation",Range(0,2)) = 0

		[Space(10)]

		[KeywordEnum(None,Solid,Gradient,Gradient3)] _RIGHT_COLOR ("Right Color (-x)", Float) = 0
		_RightColor (" ",Color) = (0.937, 0.765, 1,1)
		_RightColor1 ("Start Color",Color) = (0.937, 0.765, 1,1)
		_RightColor2 ("Middle Color",Color) = (0.737, 0.071, 0.984,1)
		_RightColor3 ("End Color",Color) = (0.737, 0.071, 0.984,1)
		[GradientPositionSliderDrawer]
		_RightColorPosition ("Position",Vector) = (0,1,0)
		_RightColorPosition3 ("Middle Size",Range(0,1)) = 0
		_RightColorRotation ("Rotation",Range(0,2)) = 0

		[Space(10)]

		[KeywordEnum(None,Solid,Gradient,Gradient3)] _LEFT_COLOR ("Left Color (+x)", Float) = 2
		_LeftColor (" ",Color) = (0.765, 0.522, 0.459,1)
		_LeftColor1 ("Start Color",Color) = (0.765, 0.522, 0.459,1)
		_LeftColor2 ("Middle Color",Color) = (0.98, 0.859, 0.706,1)
		_LeftColor3 ("End Color",Color) = (0.98, 0.859, 0.706,1)
		[GradientPositionSliderDrawer]
		_LeftColorPosition ("Gradient Position",Vector) = (0,1,0)
		_LeftColorPosition3 ("Middle Size",Range(0,1)) = 0
		_LeftColorRotation ("Gradient Rotation",Range(0,2)) = 0

		[Header(_________________________________________________________________________________________________________________________________________________________________________)]

		[Header(Fading)][Space(10)]
		[KeywordEnum(None, TOP, BOTTOM, FRONT, BACK)] _FADE ("Direction", Float) = 0
		_FadeColor ("Color",Color) = (0, 0, 0,1)
		[GradientPositionSliderDrawer]
		_FadePower ("Power",Vector) = (0,1,0)

		[Header(_________________________________________________________________________________________________________________________________________________________________________)]

		[Header(Shadow)][Space(10)]
		[Toggle] _Shadow ("Enable", Float) = 0
		_ShadowColor ("Shadow Color",Color) = (0.6, 0.5, 0.5,1)
		_ShadowPower ("Shadow Power",Range(0,1)) = 0.5


	}
	SubShader
	{					
		Pass
		{		
			Name "BASE"
			Tags { 
				"LightMode"="ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Options
			#pragma fragmentoption ARB_precision_hint_fastest        	
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#pragma multi_compile_fog
			#pragma shader_feature _MIXCOLOR_ON        	
        	#pragma shader_feature _SHADOW_ON
			#pragma shader_feature _UV_SPACE_OBJECT _UV_SPACE_WORLD _UV_SPACE_CAMERA
			#pragma shader_feature _FADE_NONE _FADE_TOP _FADE_BOTTOM _FADE_FRONT _FADE_BACK

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "AutoLight.cginc"
			#include "ColorBoxHelper.cginc"
			#include "ColorBoxCore.cginc"

			ENDCG
		}
// shadow
	    Pass {
	        Name "SHADOWCASTER"
	        Tags { "LightMode" = "ShadowCaster" }
	       
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			 
			struct v2f {
			    V2F_SHADOW_CASTER;
			};
			 
			v2f vert( appdata_base v )
			{
			    v2f o;
			    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			    return o;
			}
			 
			float4 frag( v2f i ) : SV_Target
			{
			    SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
			 
	    }

// shadow collector
	    Pass
	    {
	        Name "SHADOWCOLLECTOR"
	        Tags { "LightMode" = "ShadowCollector" }
	       
	        Fog {Mode Off}
	        ZWrite On ZTest LEqual
	 
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_shadowcollector	 	        
	        #define SHADOW_COLLECTOR_PASS
	        #include "UnityCG.cginc"	 	       

          	struct appdata { float4 vertex : POSITION;};
	        struct v2f {
	            V2F_SHADOW_COLLECTOR;
	        };	       	       
	       
	        v2f vert (appdata_base v)
	        {  
	            v2f o;	 
	            TRANSFER_SHADOW_COLLECTOR(o)
	            return o;
	        }
	 
	        float4 frag (v2f i) : COLOR
	        {
	            SHADOW_COLLECTOR_FRAGMENT(i)
	        }
	        ENDCG
	    }
	}
	CustomEditor "Digicrafts.Editor.ColorBoxShaderEditor"
}
