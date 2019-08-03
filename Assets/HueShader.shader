Shader "Unlit/HueShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_HueTex("HueTexture", 2D) = "white" {}
		_Hue("Hue", Vector) =  (0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct VS_INPUT
            {
            	float4 Position : POSITION0;
                float3 Normal	: NORMAL0;
                float3 TexCoord : TEXCOORD0;
                float3 Hue		: TEXCOORD1;
                //float4 vertex : POSITION;
               // float2 uv : TEXCOORD0;
            };

            struct PS_INPUT
            {
            	float4 Position : POSITION0;
                float3 TexCoord : TEXCOORD0;
                float3 Normal	: TEXCOORD1;
                float3 Hue		: TEXCOORD2;
               /* float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;*/
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _HueTex;
			float4 _Hue;

            PS_INPUT vert (VS_INPUT v)
            {
                PS_INPUT o;
                o.Position = UnityObjectToClipPos(v.Position);
                o.TexCoord = TRANSFORM_TEX(v.TexCoord, _MainTex);
                o.Normal = IN.Normal;
                o.Hue = IN.Hue;
                return o;
            }

            fixed4 frag (PS_INPUT i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.TexCoord);
			if (col.a <= 0)
				discard;

			float alpha = 1 - _Hue.z;

			int mode = int(_Hue.y);

			if (mode == 1)
			{
				float4 hueColor;
				//if (_Hue.x < 3000)
					hueColor = tex2D(_HueTex, float2(col.r, _Hue.x / 3000));
				//else
				//	hueColor = tex2D(_HueTex, float2(col.r, (_Hue.x - 3000) / 3000));
				hueColor.a = col.a;


				col = hueColor;
			}

				// apply fog
               // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
