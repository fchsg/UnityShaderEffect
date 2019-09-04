Shader "SG/Unlit/Texture/TextureStencil"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_StencilTex ("_Stencil (RGB), Alpha (A)", 2D) = "black" {}
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _StencilTex;
			float4 _StencilTex_ST;
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				fixed gray : TEXCOORD1; 
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata_t v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color; 
				return o;
			}
				
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 col;  
                col = tex2D(_MainTex, IN.texcoord);  

                fixed stencilA = tex2D(_StencilTex, IN.texcoord).a;
                if(stencilA > 0)  
                {  
                   col.rgb = dot(col.rgb, fixed3(0.299, 0.587, 0.114)); //grey
                }  
                else  
                {  
                   col = col * IN.color; 
                }  
                return col;  

			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
