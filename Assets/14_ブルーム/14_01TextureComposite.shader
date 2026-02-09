Shader "Custom/14_01LuminanceExtract"
{
    Properties
    {
        //辉度
        _OtherTexutre("Other Texture", 2D) = "black" {}
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

            TEXTURE2D(_OtherTexutre);
            SAMPLER(sampler_OtherTexutre);

            half4 frag(Varyings IN) : SV_Target
            {
                half4 blitColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                half4 otherColor = SAMPLE_TEXTURE2D(_OtherTexutre, sampler_OtherTexutre, IN.texcoord);

                half4 outputColor = saturate(blitColor + otherColor);
                return outputColor;
            }



            ENDHLSL
        }
    }
}
