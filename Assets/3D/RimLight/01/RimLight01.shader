Shader "SG/3D/RimLight01"{
	Properties {
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_RimColor("RimColor",Color) = (1,1,0,1)
		_RimPower ("Rim Power", Range(0.1,8)) = 3.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
			Blend SrcAlpha One
			ZWrite off
			Lighting off
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
 
			float4 _RimColor;
			sampler2D _BumpMap;
			float _RimPower;
			float4 _BumpMap_ST;
			
			struct v2f {
				float4  pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewDir:TEXCOORD1;
				float3 tangent:TEXCOORD2;
				float3 binormal:TEXCOORD3;
				float3 normal:TEXCOORD4;
			} ;
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX( v.texcoord, _BumpMap );
				o.tangent=v.tangent.xyz;
				o.binormal=cross(v.normal,v.tangent)*v.tangent.w;
				o.normal=v.normal;
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				return o;
			}
			float4 frag (v2f i) : COLOR
			{
				float3x3 rotation=float3x3 (i.tangent.xyz,i.binormal,i.normal);
//				float4 packedN=tex2D(_BumpMap,i.uv);
//				float3 N=float3(2.0*packedN.wy-1,1.0);
//				N.z=sqrt(1-N.x*N.x-N.y*N.y);
				float3 N = UnpackNormal(tex2D(_BumpMap,i.uv));
				N=normalize(mul(N,rotation));//把从贴图中得到的法线转换到世界坐标中
				float rim=1 - saturate(dot(i.viewDir,N));
				float4 c=_RimColor*pow(rim,_RimPower);
				return c; 
			}
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}