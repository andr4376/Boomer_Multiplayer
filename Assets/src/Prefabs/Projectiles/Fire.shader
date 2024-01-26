Shader "Billboard/Fire"
{
    Properties
    {
        //fire randomness
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        //fire glow
        _MaskTexture ("Mask Texture", 2D) = "white" {}
        //fire colors
		_ColorTexture("Color Texture", 2D) = "white" {}
        //scroll vector (wind direction)
		_ScrollSpeed("Scroll Speed", Vector) = (0.1,0.1,0,0)
        //pulse frequence
		_PulseSpeed("Pulse Speed", Float) = 2
        //scale for fire effect
		_BillboardScale("Billboard Scale", Float) = 3

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Pass
        {
			Blend One One //Additive Blend Mode
		    Cull Off //render all sides
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                 float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

             sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;

            sampler2D _MaskTexture;
			sampler2D _ColorTexture;

			float4 _ScrollSpeed;
			float _PulseSpeed;
			float _BillboardScale;


            v2f vert (appdata v)
            {
               
                v2f o;
				//Billboard
				float4 pos = v.vertex;

				//Only convert the pivot point to view space, then add the object space vertex position.
				float4 viewSpacePivot = float4(UnityObjectToViewPos(float4(0, 0, 0, 1)), 1);
				float4 viewSpace = viewSpacePivot + float4(pos.x, pos.y, 0, 0) * _BillboardScale;
				//Convert to clip-space
				pos = mul(UNITY_MATRIX_P, viewSpace);
				o.pos = pos;

				o.uv = v.uv;
                o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTexture);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
				//Sample noise
                float noise = tex2D(_NoiseTexture, i.noiseUV + _ScrollSpeed.xy * _Time.yy).r;
				//Sample mask
				float mask = tex2D(_MaskTexture, i.uv).r;

				//Add noise to mask to enhance the bright part of the mask, then multiply to cancel out the outer parts
				float maskCombine = saturate((noise + mask) * mask);

				float pulse = sin(_Time.y * _PulseSpeed) * 0.08;

				//Smoothly sample across the color texture based on brightness, and add pulse
				float colorSample = smoothstep(0, 1, maskCombine + pulse);
				float4 color = tex2D(_ColorTexture, colorSample);
				
				//Smoothly apply the mask with a pulse
				color *= smoothstep(0, 0.5 + pulse, mask);

               

				
				return color;

            }
            ENDCG
        }
    }
}


