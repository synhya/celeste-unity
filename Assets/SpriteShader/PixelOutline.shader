Shader "Custom/PixelOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        struct Attributes 
        {
            float4 positionOS	: POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS : NORMAL;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings 
        {
            float4 positionCS 	: SV_POSITION;
            float2 uv : TEXCOORD0;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
            // UNITY_VERTEX_OUTPUT_STEREO
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;

        float4 _Color;

        float _Radius;

        Varyings vert (Attributes input)
        {
            Varyings output;

            // UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_TRANSFER_INSTANCE_ID(input, output);
            // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);

            output.positionCS = positionInputs.positionCS;
            output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            return output;
        }

        float4 frag (Varyings input) : SV_Target
        {
            // UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float na = 0;
            float r = _Radius;

            for (int nx = -r; nx <= r; nx++)
            {
                for(int ny = -r; ny <= r; ny++)
                {
                    if(nx * nx + ny * ny <= r) // r * r is ugly
                    {
                        float4 nc = tex2D(_MainTex, input.uv +
                            float2(_MainTex_TexelSize.x * nx, _MainTex_TexelSize.y * ny));
                        na += ceil(nc.a);
                        // if that texture is filled with color
                        // na increases
                    }
                }
            }

            na = clamp(na, 0, 1);

            float4 c = tex2D(_MainTex, input.uv);
            na -= ceil(c.a);

            return lerp(c, _Color, na);
        }
        ENDHLSL

        Pass
        {
            Tags{"LightMode"="SRPDefaultUnlit"}
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            
            ENDHLSL
        }

//        Pass
//        {
//            Tags{"LightMode"="ShadowCaster"}
//            ZWrite On
//            ZTest LEqual
//            ColorMask 0
//            
//            HLSLPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            
//            #pragma  multi_compile_shadowcaster
//            
//            ENDHLSL
//        }
//
//        Pass
//        {
//            Tags 
//            {
//                "LightMode"="DepthOnly"
//            }
//            
//            HLSLPROGRAM
//
//            #pragma vertex vert
//            #pragma fragment frag
//
//            ENDHLSL
//        }
//         
//        Pass
//        {
//            Tags 
//            {
//                "LightMode"="DepthNormalsOnly"
//            }
//            HLSLPROGRAM
//
//            #pragma vertex vert
//            #pragma fragment frag
//
//            ENDHLSL
//        }
    }
}
