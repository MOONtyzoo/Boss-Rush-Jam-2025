/*
    https://www.youtube.com/watch?v=kiSKb54cogo

    Code for hue shifting, saturation, and brightness come from this tutorial video
*/

Shader "Unlit/SpriteEffects"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        _TintAmount ("Tint Amount", Range(0.0, 1.0)) = 0
        _HueShift ("Hue Shift", Range(0, 6.28318531)) = 0
        _Saturation ("Saturation", Range(0, 5)) = 1
        _Brightness ("Brightness", Range(-1, 1)) = 0
        _Opacity ("Opacity", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" } // Transparent meshes are rendered after opaque meshes
        Blend SrcAlpha OneMinusSrcAlpha // Blend source and destination colors based on transparency
        //ZTest Off // If ZTest is on the shader will override Unity's sorting layer for some reason
        Cull Off // Disable backfacing culling so that sprite can be flipped

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.14159265359

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


            /*
                Vertex Shader
            */
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Model-View-Projection
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            /*
                Fragment Shader
            */

            float4 _TintColor;
            float _TintAmount;
            
            float _HueShift;
            float _Saturation;
            float _Brightness;
            float _Opacity;

            float3 ApplyColorEffects(float3 color, float hueShift, float saturation, float brightness)
            {
                /*
                    See https://en.wikipedia.org/wiki/YIQ
                    for explanation of color space conversions

                    Converting to YIQ instead of HSV preserves perceived brightness of color.
                */

                float3x3 RGB_TO_YIQ = 
                    float3x3 (0.299, 0.587, 0.114,
                              0.5959, -0.2756, -0.3213,
                              0.2115, -0.5227, 0.3112);
                
                float3x3 YIQ_TO_RBG = 
                    float3x3 (1, 0.956, 0.619,
                            1, -0.272, -0.647,
                            1, -1.106, 1.703);
                
                // Convert RBG color to YIQ space
                float3 YIQ = mul(RGB_TO_YIQ, color);

                // "Shift" hue by rotating IQ vector (this preserves Y, which is luminance)
                float hue = atan2(YIQ.z, YIQ.y) + _HueShift;
                // Chroma is saturation? Y encodes only lightness so length of IQ must be the colorful parts
                float chroma = length(float2(YIQ.y, YIQ.z)) * _Saturation;
                // hue is the angle of the IQ vector, chroma is the length, so we reconstruct the new vector after shifting hue
                float3 shiftedYIQ = float3(YIQ.x + _Brightness, chroma*cos(hue), chroma*sin(hue));

                // Convert YIQ space to RGB space
                float3 shiftedRGB = mul(YIQ_TO_RBG, shiftedYIQ);
                // VERY IMPORTANT!!! The YIQ_TO_RGB matrix might kick the colors outside of the expected 0-1 range resulting in choppy edges
                shiftedRGB = clamp(shiftedRGB, float3(0, 0, 0), float3(1, 1, 1));
                
                return shiftedRGB;
            }

            float3 ApplyTintColor(float3 mainColor, float3 tintColor, float tintAmount) {
                tintAmount = clamp(tintAmount, 0.0, 1.0);
                return mainColor*(1.0-tintAmount) + tintColor*(tintAmount);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = ApplyColorEffects(col.rgb, _HueShift, _Saturation, _Brightness);
                col.rgb = ApplyTintColor(col.rgb, _TintColor.rgb, _TintAmount);
                col.a = min(col.a, _Opacity);
                return col;
            }
            ENDCG
        }
    }
}
