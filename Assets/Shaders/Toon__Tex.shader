 Shader "Custom/Toon_texture"
{
	Properties
	{
		_AmbientColor("Ambient Color", Color) = (1,1,1,1)
	_MainTex ("Texture", 2D) = "white" {}
	_SpecularPower("Specular Power", Float)=47
	_SpecularStrenght("Specular strenght", Range(0,1))=0.5
	_RimColor("Rom Color",Color) = (1,1,1,1)
	_RimStrenght	("Rim Strenght", Range(0,1))=1
	_RimThreshold		("Rim Threshold", Range(0,1))=1
	_RimAmount		("Rim Amount", Range(0,1))=1

	_LightIntensity("Light Intensity",Range(-0.5,0.5)) = 0
	_ShadowSoftness("Shadow Softness", Range(0,0.02)) =0.01


	}
		SubShader
	{

		Tags {	"RenderType" = "Opaque" 
	"LightMode" = "ForwardBase"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Assets/Shaders/Fog.cginc"
			#include "UnityLightingCommon.cginc"
			#include "AutoLight.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float3 viewDirection : TEXCOORD2;
				float3 normal : NORMAL;
float2 uv : TEXCOORD0;
				SHADOW_COORDS(3)
			};

			float4 _AmbientColor;

			sampler2D _MainTex;
            float4 _MainTex_ST;

			float _SpecularPower;
			float _SpecularStrenght;//glow spot intensity

			float _ShadowSoftness;

			//Rim lighting
			float _RimStrenght;
			float _RimAmount;
			float4 _RimColor;
			float _RimThreshold;
			float _LightIntensity;
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				//WORLD SPACE
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				o.normal = UnityObjectToWorldNormal(v.normal);

				o.viewDirection = WorldSpaceViewDir(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//shadow
				TRANSFER_SHADOW(o)

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

			//lighting
				float3 normal = normalize(i.normal);
				float3 viewDirection = normalize(i.viewDirection);

				float nDotL = dot(_WorldSpaceLightPos0, normal);//diffuse dot product

				float diffuse = max(0,nDotL) * SHADOW_ATTENUATION(i);

				diffuse = max(_ShadowSoftness,diffuse);

				//toon (Very sharp shadow edge)
				diffuse = smoothstep(0,0.02,diffuse); //adjust second val


				//Specular lighting
				float3 r = -reflect(_WorldSpaceLightPos0, normal);
				float rDotV = max(0,dot(r, viewDirection));
				float specular = pow(rDotV, _SpecularPower);

				//toon
				specular = smoothstep(0,0.2,specular)*_SpecularStrenght; 
				
				//Rim
				float rimDot = 1-dot(viewDirection,normal);
				float rimIntensity = rimDot * pow(nDotL,_RimThreshold );
				//sharp edges
				rimIntensity = smoothstep(_RimAmount-0.01, _RimAmount+0.01, rimIntensity) * _RimStrenght;

				float4 rim = rimIntensity*_RimColor;
				
				//
				// sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

				//apply ligthing
				float4 color = diffuse * ((_AmbientColor*col) +
				 ((_LightColor0 * _LightIntensity) +//+ for high exposure * for darker 
				  specular + rim)); 

				 color= ApplyFog(color, i.worldPos);

				return color;

			}
			ENDCG
		}
	}
		//allow shadow castiong from standard shaders
				Fallback "Diffuse"
}
