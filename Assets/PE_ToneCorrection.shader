Shader "CustomRenderTexture/ToneCorrection"
{
    Properties
    {
        saturation("彩度", range(0, 1)) = 1
        contrast("对比度", range(0, 2)) = 1
    }
     SubShader
     {
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            ZWrite Off
            Cull Off
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma editor_sync_compilation

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            half saturation;
            half contrast;
           half4 Frag(Varyings input) : SV_Target
           {
               half4 output = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearRepeat,input.texcoord);

                half grayscale = 
                    0.2126 * output.r +
                    0.7152 * output.g +
                    0.0722 * output.b;
                    half4 monochromeColor = half4(grayscale, grayscale, grayscale, 1);
                    half4 outputColor = lerp(monochromeColor, output, saturation);
                    outputColor.rgb = (outputColor.rgb - 0.5) * contrast + 0.5;
                    
                return outputColor;
            
           }
            ENDHLSL
        }
    }
}
