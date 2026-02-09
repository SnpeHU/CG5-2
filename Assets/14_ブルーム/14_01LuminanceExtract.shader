Shader "Custom/14_01LuminanceExtract"
{
    Properties
    {
        _ThresholdMin ("Threshold Min", Range(0,2)) = 1.0
        _ThresholdMax ("Threshold Max", Range(0,2)) = 1.5
    }

    SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag

            // Enable editor sync compilation to catch shader errors in the editor
            #pragma editor_sync_compilation

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

           CBUFFER_START(UnityPerMaterial)
                float _ThresholdMin;
                float _ThresholdMax;
            CBUFFER_END
           
           half4 frag(Varyings IN) : SV_Target
           {
                half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearRepeat, IN.texcoord);
                
                half luminance = color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722;
                luminance = smoothstep(_ThresholdMin, _ThresholdMax, luminance);

                half4 outputColor = color * luminance;
                outputColor.a = 1.0;
                return outputColor;
           }


            ENDHLSL
        }
    }
}
