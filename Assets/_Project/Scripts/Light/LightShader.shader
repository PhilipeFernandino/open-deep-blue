Shader "Custom/LightOverlay" {
    Properties {
        _LightTex ("Light Texture", 2D) = "white" {}
        _DarkColor ("Darkness Color", Color) = (0,0,0,1)
        _Opacity ("Opacity", Range(0,1)) = 0.8
    }

    SubShader {
        Tags { 
            "Queue"="Transparent+100"  // Render after all other objects
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha  // Alpha blending
        ZWrite Off  // Disable depth writing

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _LightTex;
            float4 _DarkColor;
            float _Opacity;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Get light value from texture
                float light = tex2D(_LightTex, i.screenPos.xy / i.screenPos.w).r;

                // Blend between darkness and full visibility
                fixed4 col = lerp(_DarkColor, fixed4(1,1,1,1), light);
                col.a = _Opacity * (1 - light);  // Darker areas more opaque
                return col;
            }
            ENDCG
        }
    }
}