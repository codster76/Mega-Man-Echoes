Shader "Unlit/MegaManShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ReplacementMap ("Colour Palette", 2D) = "white" {}
		_Flip ("Flip", Vector) = (1,1,1,1)
        _Offset ("Palette Number", Float) = 0.0
    }
    SubShader
    {
        Tags {"RenderType"="Transparent" "Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Zwrite Off

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
			sampler2D _ReplacementMap; // A texture that's 255x1 in size. Colour pixels at coordinates based on the r values on the original texture
            float4 _MainTex_ST;
            float _Offset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				// code idea from https://forum.unity.com/threads/color-replacement-shader.83582/
			
				// get the red value of the original texture
				float coord = tex2D(_MainTex, i.uv).r;
			
                // get the current pixel colour from the main texture
                float4 col = tex2D(_MainTex, i.uv);
				
				// using the red value as a coordinate, get the pixel at that x value from the second image
				col.rgb = tex2D(_ReplacementMap, float2(coord + ((_Offset*4)/255), 0)).rgb;
                //col.rgb = tex2D(_ReplacementMap, float2(coord, 0)).rgb;
				
                return col * col.a;
            }
            ENDCG
        }
    }
}
