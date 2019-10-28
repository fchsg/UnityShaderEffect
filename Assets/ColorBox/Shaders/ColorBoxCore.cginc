// Input stucture
struct appdata
{				
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float2 uv :  TEXCOORD0;
	float2 uv2 :  TEXCOORD1;
	float4 tangent : TANGENT;
	#if _VERTEX_COLOR_ON
	float4 color : COLOR;
	#endif
};
// Output stucture
struct v2f
{		
	float4 pos : SV_POSITION;		                
    float3 coords : TEXCOORD0;
	half3 objNormal : TEXCOORD1;
	half3 worldNormal : TEXCOORD2;
	UNITY_FOG_COORDS(3)

	#if _TEXTUREENABLED_ON
	float2 uv : TEXCOORD4;
	#endif

	#if _VERTEX_COLOR_ON
	fixed3 color : COLOR0;
	#endif

	#if _DIFFUSE_ON
	fixed3 diff : COLOR1;
	#endif

	#if _AMBIENT_ON
    fixed3 ambient : COLOR2;
    #endif

    #if _SPECULAR_ON
	float3 normalDir : COLOR3;
	#endif

    #if _SPECULAR_ON || _UV_SPACE_WORLD || _UV_SPACE_CAMERA
    float3 worldPos : TEXCOORD5;
    #endif

    #if _UV_SPACE_WORLD
	float3 worldViewDir : TEXCOORD6;
	#elif _UV_SPACE_SCREEN	
	float4 screenPos : TEXCOORD7;
	#elif _UV_SPACE_CAMERA	
	float3 worldViewDir : TEXCOORD6;
	float4 screenPos : TEXCOORD7;
	#endif

	#if _SHADOW_ON
	SHADOW_COORDS(8)
    #endif
                    			
	#if _LIGHTMAP_ON
	float2 uv2 : TEXCOORD9;
	#endif

};

#if _TEXTUREENABLED_ON
uniform half _UVSec;
uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform float _TexturePower;
#endif

uniform fixed _TOP_COLOR;
uniform fixed _BOTTOM_COLOR;
uniform fixed _LEFT_COLOR;
uniform fixed _RIGHT_COLOR;
uniform fixed _FRONT_COLOR;
uniform fixed _BACK_COLOR;

uniform fixed3 _DefaultColor;
uniform fixed3 _TopColor;
uniform fixed3 _BottomColor;
uniform fixed3 _FrontColor;
uniform fixed3 _BackColor;
uniform fixed3 _LeftColor;
uniform fixed3 _RightColor;

uniform fixed3 _TopColor1;
uniform fixed3 _TopColor2;
uniform fixed3 _TopColor3;
uniform float _TopColorRotation;
uniform float2 _TopColorPosition;
uniform float _TopColorPosition3;
uniform fixed3 _BottomColor1;
uniform fixed3 _BottomColor2;
uniform fixed3 _BottomColor3;
uniform float _BottomColorRotation;
uniform float2 _BottomColorPosition;
uniform float _BottomColorPosition3;
uniform fixed3 _FrontColor1;
uniform fixed3 _FrontColor2;
uniform fixed3 _FrontColor3;
uniform float _FrontColorRotation;
uniform float2 _FrontColorPosition;
uniform float _FrontColorPosition3;
uniform fixed3 _BackColor1;
uniform fixed3 _BackColor2;
uniform fixed3 _BackColor3;
uniform float _BackColorRotation;
uniform float2 _BackColorPosition;
uniform float _BackColorPosition3;
uniform fixed3 _LeftColor1;
uniform fixed3 _LeftColor2;
uniform fixed3 _LeftColor3;
uniform float _LeftColorRotation;
uniform float2 _LeftColorPosition;
uniform float _LeftColorPosition3;
uniform fixed3 _RightColor1;
uniform fixed3 _RightColor2;
uniform fixed3 _RightColor3;
uniform float _RightColorRotation;
uniform float2 _RightColorPosition;
uniform float _RightColorPosition3;

uniform float _MixColorPower;
uniform float2 _FadePower;
uniform fixed3 _FadeColor;


#if _DIFFUSE_ON
uniform fixed4 _DiffuseColor;
uniform float _DiffusePower;
#endif

#if _AMBIENT_ON
uniform float _AmbientPower;
uniform fixed4 _AmbientColor;
#endif

#if _SHADOW_ON
uniform fixed4 _ShadowColor;
uniform float _ShadowPower;
#endif

#if _LIGHTMAP_ON
uniform float _LightmapPower;
#endif

#if _SPECULAR_ON
//			uniform fixed4 _SpecColor;
uniform float _Shininess;
#endif

// Vertex Function
v2f vert (appdata v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.objNormal = v.normal;
	o.worldNormal = UnityObjectToWorldNormal(v.normal);
//	o.coords = v.color.rgb;
	o.coords=v.tangent.xyz;

	#if _VERTEX_COLOR_ON
	o.color=v.color.rgb;
	#endif

	#if _TEXTUREENABLED_ON
	if(_UVSec==0)
	o.uv = TRANSFORM_TEX(v.uv,_MainTex);
	else
	o.uv = TRANSFORM_TEX(v.uv2,_MainTex);
	#endif

	#if _DIFFUSE_ON
		half nl = max(0, dot(o.worldNormal, _WorldSpaceLightPos0.xyz));
		o.diff = nl * _LightColor0 * _DiffusePower * _DiffuseColor;
	#endif

	#if _AMBIENT_ON
		o.ambient = ShadeSH9(half4(o.worldNormal,1)) * _AmbientPower;
	#endif

	#if _SHADOW_ON
		TRANSFER_SHADOW(o)
	#endif

	#if _LIGHTMAP_ON					
		o.uv2 = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif

	#if _SPECULAR_ON || _UV_SPACE_WORLD || _UV_SPACE_CAMERA
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
	#endif
	#if _UV_SPACE_WORLD
		o.worldViewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
	#elif _UV_SPACE_SCREEN
		o.screenPos = ComputeScreenPos(o.pos);
	#elif _UV_SPACE_CAMERA
		o.worldViewDir = mul((float3x3)UNITY_MATRIX_V, o.worldNormal);
		o.screenPos = ComputeScreenPos(o.pos);
	#endif

	#if _SPECULAR_ON
	o.normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
	#endif

	UNITY_TRANSFER_FOG(o,o.pos);

	return o;
}

// Fragment Function
fixed4 frag (v2f i) : SV_Target
{								

	#ifdef _UV_SPACE_WORLD	                		
    	half3 blend = abs(i.worldNormal);	                
		float3 uv = 0.5-i.worldViewDir;
		half3 n = i.worldNormal;
	#elif _UV_SPACE_SCREEN	
    	half3 blend = abs(i.worldNormal);
		float3 uv =  float3((i.screenPos.xy / i.screenPos.w - 0.5)*2,0);
		uv.z=uv.y;
		half3 n = i.worldNormal;
	#elif _UV_SPACE_CAMERA	
		float3 k = abs(i.worldViewDir);
    	half3 blend = abs(k);
		float3 uv =  float3((i.screenPos.xy / i.screenPos.w - 0.5)*2,0);
		uv.z=uv.y;
		half3 n = i.worldViewDir;
	#else						                		
        half3 blend = abs(i.objNormal);
        float3 uv = i.coords;
        half3 n = i.objNormal;
	#endif // _UV_SPACE_WORLD

	// Top/Bottom Color
	fixed3 cy;
	if(n.y>0) {
		if(_TOP_COLOR==1)
			cy = _TopColor.rgb;
		else if(_TOP_COLOR==2)
			cy = lerp(_TopColor3,_TopColor1,clampValue(rotateUV(uv.xz,_TopColorRotation*PI).y,_TopColorPosition));    					    				
		else if(_TOP_COLOR==3)
			cy = lerp3(_TopColor3,_TopColor2,_TopColor1,clampValue(rotateUV(uv.xz,_TopColorRotation*PI).y,_TopColorPosition), _TopColorPosition3);
		else
			cy = _DefaultColor.rgb;
	} else {
		if(_BOTTOM_COLOR==1)
			cy = _BottomColor.rgb;							    				
		else if(_BOTTOM_COLOR==2)
			cy = lerp(_BottomColor3,_BottomColor1,clampValue(rotateUV(uv.xz,_BottomColorRotation*PI).y,_BottomColorPosition));
		else if(_BOTTOM_COLOR==3)
			cy = lerp3(_BottomColor3,_BottomColor2,_BottomColor1,clampValue(rotateUV(uv.xz,_BottomColorRotation*PI).y,_BottomColorPosition),_BottomColorPosition3);
		else
			cy = _DefaultColor.rgb;		        
	}

	// Front/Back Color
	fixed3 cz;
	if(n.z>0) {
		if(_BACK_COLOR==1)
			cz = _BackColor.rgb;
		else if(_BACK_COLOR==2)
			cz = lerp(_BackColor3,_BackColor1,clampValue(rotateUV(uv.xy,_BackColorRotation*PI).y,_BackColorPosition));    					    				
		else if(_BACK_COLOR==3)
			cz = lerp3(_BackColor3,_BackColor2,_BackColor1,clampValue(rotateUV(uv.xy,_BackColorRotation*PI).y,_BackColorPosition),_BackColorPosition3);
		else
			cz = _DefaultColor.rgb;
	} else {
		if(_FRONT_COLOR==1)
			cz = _FrontColor.rgb;							    				
		else if(_FRONT_COLOR==2)
			cz = lerp(_FrontColor3,_FrontColor1,clampValue(rotateUV(uv.xy,_FrontColorRotation*PI).y,_FrontColorPosition));
		else if(_FRONT_COLOR==3)
			cz = lerp3(_FrontColor3,_FrontColor2,_FrontColor1,clampValue(rotateUV(uv.xy,_FrontColorRotation*PI).y,_FrontColorPosition),_FrontColorPosition3);
		else
			cz = _DefaultColor.rgb;		        
	}

	// Left/Right Color
	fixed3 cx;
	if(n.x>0) {
		if(_LEFT_COLOR==1)
			cx = _LeftColor.rgb;
		else if(_LEFT_COLOR==2)
			cx = lerp(_LeftColor3,_LeftColor1,clampValue(rotateUV(uv.zy,_LeftColorRotation*PI).y,_LeftColorPosition));
		else if(_LEFT_COLOR==3)
			cx = lerp3(_LeftColor3,_LeftColor2,_LeftColor1,clampValue(rotateUV(uv.zy,_LeftColorRotation*PI).y,_LeftColorPosition),_LeftColorPosition3);
		else
			cx = _DefaultColor.rgb;
	} else {
		if(_RIGHT_COLOR==1)
			cx = _RightColor.rgb;
		else if(_RIGHT_COLOR==2)
			cx = lerp(_RightColor3,_RightColor1,clampValue(rotateUV(uv.zy,_RightColorRotation*PI).y,_RightColorPosition));
		else if(_RIGHT_COLOR==3)
			cx = lerp3(_RightColor3,_RightColor2,_RightColor1,clampValue(rotateUV(uv.zy,_RightColorRotation*PI).y,_RightColorPosition),_RightColorPosition3);
		else
			cx = _DefaultColor.rgb;	
	}
    
    blend /= blend.x+blend.y+blend.z;//dot(blend,1.0);
                           	
    // Check if need to mix the color
    #ifdef _MIXCOLOR_ON
		fixed3 c = fixed3(cx * blend.x + cy * blend.y + cz * blend.z);
    #else
		fixed3 c;
		if(_MixColorPower==1){
			c = fixed3(cx * blend.x + cy * blend.y + cz * blend.z);
		} else {
			if(blend.x>blend.y){
				if(blend.x>blend.z){
					c = cx *(1-_MixColorPower) + fixed3(cx * blend.x + cy * blend.y + cz * blend.z)*_MixColorPower;
				} else {
					c=cz*(1-_MixColorPower) + fixed3(cx * blend.x + cy * blend.y + cz * blend.z)*_MixColorPower;
				}
			} else {
				if(blend.y>blend.z){
					c=cy*(1-_MixColorPower) + fixed3(cx * blend.x + cy * blend.y + cz * blend.z)*_MixColorPower;
				} else {
					c=cz*(1-_MixColorPower) + fixed3(cx * blend.x + cy * blend.y + cz * blend.z)*_MixColorPower;
				}
			}
		}
	#endif

	#if _SHADOW_ON
		fixed shadow = clamp(SHADOW_ATTENUATION(i)+1-_ShadowPower,0,1);
	#else
		fixed shadow = 1;
	#endif


	#if _TEXTUREENABLED_ON					

		fixed3 texColor = tex2D (_MainTex, i.uv).rgb;
		fixed lux = dot(texColor, fixed3(0.2126, 0.7152, 0.0722));
		// The actual Overlay/High Light method is based on the shader
    	if (lux < 0.5) {
        	c = 2.0 * texColor * c;
    	} else {
            // We need a white base for a lot of the algorith,
            fixed3 white = fixed3(1,1,1);
            c = white - 2.0 * (white - texColor) * (white - c);
    	}
		
	#endif


	#if _VERTEX_COLOR_ON
	c=c*i.color;
	#endif

    // return color
    #if _DIFFUSE_ON && _AMBIENT_ON
    	c=c*(i.diff + 1-_DiffusePower)*shadow+i.ambient*_AmbientColor;					
	#else
		#if _DIFFUSE_ON
			c=c*shadow*(i.diff + 1-_DiffusePower);
		#else 
			#if _AMBIENT_ON
				c=c*shadow+i.ambient*_AmbientColor;
			#else
				c=c*shadow;
			#endif
		#endif
	#endif									
				

	#if _SHADOW_ON
		c = c + (1-shadow)*_ShadowColor.rgb*_ShadowPower;
	#endif

	#if _SPECULAR_ON
		float3 normalDirection = normalize(i.normalDir); 
    	float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
    	float3 lightDirection;
    	float attenuation;

        if (0.0 == _WorldSpaceLightPos0.w) // directional light?
        {
           attenuation = 1.0; // no attenuation
           lightDirection = normalize(_WorldSpaceLightPos0.xyz);
        } 
        else // point or spot light
        {
           float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.worldPos.xyz;
           float distance = length(vertexToLightSource);
           attenuation = 1.0 / distance; // linear attenuation 
           lightDirection = normalize(vertexToLightSource);
        }

    	float3 diffuseReflection = attenuation * _LightColor0.rgb *  _SpecColor.rgb * max(0.0, dot(normalDirection, lightDirection));

		float3 specularReflection;
    	if (dot(normalDirection, lightDirection) < 0.0) 
       	// light source on the wrong side?
    	{
       		specularReflection = float3(0.0, 0.0, 0.0); 
		// no specular reflection
    	}
    	else // light source on the right side
    	{
   		specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection))*_Shininess;// * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), (10-_Shininess*10));
    	}
    	c = c + specularReflection;
	#endif

	#if _LIGHTMAP_ON
	#ifdef LIGHTMAP_ON
		c += DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2))*_LightmapPower;//fixed3(i.uv2,0);//				
	#endif
	#endif

	#if _FADE_TOP
		float a = clampValue(0.5-i.coords.y,_FadePower);
		c = c*a + _FadeColor*(1-a);
	#elif _FADE_BOTTOM
		float a = clampValue(0.5-i.coords.y,_FadePower);
		c = c*(1-a) + _FadeColor*a;
	#elif _FADE_FRONT
		float a = clampValue(0.5-i.coords.z,_FadePower);
		c = c*(1-a) + _FadeColor*a;
	#elif _FADE_BACK
		float a = clampValue(0.5-i.coords.z,_FadePower);
		c = c*a + _FadeColor*(1-a);
	#endif

	#if _DEBUG_X
		c = fixed3(abs(uv.x),abs(uv.x),abs(uv.x));
	#elif _DEBUG_Y
		c = fixed3(abs(uv.y),abs(uv.y),abs(uv.y));
	#elif _DEBUG_Z
		c = fixed3(abs(uv.z),abs(uv.z),abs(uv.z));
	#elif _DEBUG_ALL
		c = fixed3(abs(uv.xyz));
	#endif

	// Apply fog
	fixed4 final = fixed4(c,1); 
	UNITY_APPLY_FOG(i.fogCoord,final);
	
	return final;				
}