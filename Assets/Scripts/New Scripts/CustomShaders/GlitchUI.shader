Shader "UI/GlitchUIFixed"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _GlitchStrength ("Glitch Strength", Range(0,1)) = 0.5
        _OffsetAmount ("Color Offset", Range(0,10)) = 3
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.3
        _NoiseSpeed ("Noise Speed", Range(0,10)) = 3
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _GlitchStrength;
            float _OffsetAmount;
            float _ScanlineIntensity;
            float _NoiseSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            float rand(float2 n)
            {
                return frac(sin(dot(n, float2(12.9898,78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noise = rand(float2(i.uv.y * _NoiseSpeed, _Time.y));
                float glitch = (noise - 0.5) * _GlitchStrength * 0.05;

                float2 uvR = i.uv + float2(glitch * _OffsetAmount, 0);
                float2 uvG = i.uv;
                float2 uvB = i.uv - float2(glitch * _OffsetAmount, 0);

                fixed4 col;
                fixed4 r = tex2D(_MainTex, uvR);
                fixed4 g = tex2D(_MainTex, uvG);
                fixed4 b = tex2D(_MainTex, uvB);

                col.r = r.r;
                col.g = g.g;
                col.b = b.b;

                // make sure alpha comes from original texture
                col.a = g.a;

                // tint from UI Image component
                col *= i.color;

                // scanline effect
                float scan = sin(i.uv.y * 1200) * _ScanlineIntensity * _GlitchStrength;
                col.rgb -= scan;

                return col;
            }
            ENDCG
        }
    }
}
