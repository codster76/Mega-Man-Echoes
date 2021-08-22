Shader "Unlit/MegaManShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ReplacementMap ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Blend One OneMinusSrcAlpha

        Pass
        {
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _ReplacementMap;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				// check the red value of the original texture
				// code idea from https://forum.unity.com/threads/color-replacement-shader.83582/
				//float2 test = i.uv;
				//test.x = test.x;
				//test.y = test.y + 0;
				//float coord = tex2D(_MainTex, test).r;
				float coord = tex2D(_MainTex, i.uv).r;
			
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
				col.rgb = tex2D(_ReplacementMap, float2(coord - 0.001955, 0)).rgb;
                return col * col.a;
            }
			
			// Note: I'll take in a 256 wide texture with the potential colour replacements and 
			
            ENDCG
        }
    }
}
