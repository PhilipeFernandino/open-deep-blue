Shader "Custom/LightOverlay" {
    Properties {
        _LightTex ("Light Texture", 2D) = "white" {}
        _MapSize ("Map Size", Vector) = (1024, 1024, 0, 0)
        _MapOffset ("Map Offset", Vector) = (0, 0, 0, 0)
        _LightColor ("Light Color", Color) = (1, 0.8, 0.5, 1) 
        _LightIntensity ("Light Intensity", Float) = 1.0
        _FallofIntensity ("Fallof Intensity", Float) = 0.2
        _Origin ("Origin", Vector) = (0, 0, 0, 0)
    }

    SubShader {
        Tags { 
            "Queue"="Transparent+100"  
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Blend DstColor Zero 
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            sampler2D _LightTex;
            float4 _DarkColor;
            float _Opacity;
            float4 _MapSize;
            float4 _MapOffset;
            float4 _LightColor;
            float _LightIntensity;
            float _FallofIntensity;
            float4 _Origin;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz - _Origin.xyz; // World position
                return o;
            }


            fixed4 frag (v2f i) : SV_Target {
                // Convert world position to UVs
                float2 uv = i.worldPos.xy / _MapSize.xy;

                // Sample light texture and apply tint/intensity
                float light = tex2D(_LightTex, uv).r;
                fixed4 lightTint = _LightColor * light * _LightIntensity;

                // Optional: Add falloff for smoother edges
                lightTint *= smoothstep(0, _FallofIntensity, light);

                return lightTint;
            }
            ENDCG
        }
    }
}