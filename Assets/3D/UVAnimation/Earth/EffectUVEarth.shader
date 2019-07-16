Shader "SG/Effect/UV/EffectUVEarth"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        _Speed("Speed", Range(0, 1)) = 0.5
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            CGPROGRAM
         
            #pragma vertex vert 
            #pragma fragment frag 
            #include "UnityCG.cginc"
         
            float4 _Color;
            sampler2D _MainTex; 
           
           struct v2f{
                float4 pos: SV_POSITION;
                float2 uv: TEXCOORD0;
           };
           
           float4 _MainTex_ST;
           half _Speed;
           
           v2f vert(appdata_base v){
                
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                return o;
           }
           
           half4 frag(v2f i) : COLOR{
           
           float u = i.uv.x - _Speed*_Time;
           float2 uv = float2(u, i.uv.y);
           
           return tex2D(_MainTex, uv);
           
           };
           
           ENDCG
      
        }
    }
    FallBack "Diffuse"
}
