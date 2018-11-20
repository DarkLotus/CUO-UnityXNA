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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				//float3 hue: TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
			//	float3 hue: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _HueTex;
			float4 _Hue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//o.hue = v.hue;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
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
