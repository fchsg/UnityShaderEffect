Shader "SG/3D/RimLight02"{
Properties {
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_RimColor("RimColor",Color) = (1,1,0,1)
	_RimPower ("Rim Power", Range(0.1,8)) = 3.0
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	
	CGPROGRAM
	#pragma surface surf Lambert  alpha:blend
 
	float4 _RimColor;
	float _RimPower;
	sampler2D _BumpMap;		
	struct Input {
		float2 uv_BumpMap;
		float3 viewDir;
	};
 
	void surf (Input IN, inout SurfaceOutput o) {	
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));	             
        half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
        float4 c=_RimColor * pow (rim, _RimPower); 
        o.Alpha=c.a;                 
        o.Emission =c.rgb*2;  
	}	
 
	ENDCG		
} 
FallBack "Diffuse"
}