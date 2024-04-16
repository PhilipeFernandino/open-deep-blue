// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Light2D" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            uint _TileSize = 16;
            float2 _Position = (-270, 778);

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            v2f vertex (appdata v) {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            // uv se refere 
            // 

            fixed4 fragment (v2f data) : SV_Target {
                fixed4 color;

                //for (int j = 0; j < 10; j++) {
                //    if (i.uv.x > _CoordArray[j].x && i.uv.x < _CoordArray[j].y &&
                //        i.uv.y > _CoordArray[j].z && i.uv.y < _CoordArray[j].w) {
                //        col.rgb = 0.0;
                //        break;
                //    }
                //}

                //if (i.uv.y > 0 && i.uv.y < 0.7)
                //    col.rgb

                //color.rgb = inv_color.rgb;
                //color.rgb = data.uv.y * data.uv.x * color.rgb; 
                
                color.rgb = 1 - distance(data.worldPos.xy, _Position);

                return color;
            }
            ENDCG
        }
    }
}