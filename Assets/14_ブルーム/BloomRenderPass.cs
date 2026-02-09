using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
[CreateAssetMenu(fileName = "BloomRenderPass", menuName = "Scriptable Objects/BloomRenderPass")]
public class BloomRenderPass : ScriptableRenderPass
{
    //模糊材质
    private Material blurmaterial = null;

    //辉度提取材质
    private Material luminanceExtractMaterial = null;

    //合成材质
    private Material compositeTextureMaterial = null;

    static readonly int luminanceBlurTexID = Shader.PropertyToID("_OtherTexture");

    class CompositePassData
    {
        //传给_BlitTexture的纹理
        public TextureHandle sourceTexture;

        //合成用的纹理
        public TextureHandle otherTexture;

        //输出纹理
        public TextureHandle destinationTexture;

        //
        public Material material;

    }
    public BloomRenderPass(Material luminanceExtractMat, Material blurMat, Material compositeMat)
    {
        luminanceExtractMaterial = luminanceExtractMat;
        blurmaterial = blurMat;
        compositeTextureMaterial = compositeMat;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if(luminanceExtractMaterial == null || blurmaterial == null || compositeTextureMaterial == null)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();

        //如果当前渲染目标是后备缓冲区，则不进行任何处理，直接调用基类的方法
        if(resourcesData.isActiveTargetBackBuffer)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        //获取当前相机颜色纹理
        TextureHandle cameraTexture = resourcesData.activeColorTexture;
        //获取当前相机颜色纹理的描述
        TextureDesc originalTextureDesc = renderGraph.GetTextureDesc(cameraTexture);
        originalTextureDesc.name = "Bloom Original Texture";
        originalTextureDesc.depthBufferBits = 0;

        //创建一个临时纹理用于存储原始图像
        TextureHandle orignalTempTexture = renderGraph.CreateTexture(originalTextureDesc);  

        //辉度提取
        TextureDesc luminanceExtractDesc = originalTextureDesc;
        luminanceExtractDesc.name = "_SmallTempTexture";
        int div = 4;//缩小4倍
        luminanceExtractDesc.width /= div;
        luminanceExtractDesc.height /= div;

        luminanceExtractDesc.format = 
            UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;

        //创建一个临时纹理用于存储辉度提取结果
        TextureHandle luminanceExtractTexture = renderGraph.CreateTexture(luminanceExtractDesc);

        TextureHandle luminanceBlurTexture = renderGraph.CreateTexture(luminanceExtractDesc);

        RenderGraphUtils.BlitMaterialParameters luminanceExtractBlitParams = 
        new RenderGraphUtils.BlitMaterialParameters()
        {
            source = cameraTexture,
            destination = luminanceExtractTexture,
            material = luminanceExtractMaterial
        };

        renderGraph.AddBlitPass(
            luminanceExtractBlitParams,
            "Luminance Extract Blit"
        );

        RenderGraphUtils.BlitMaterialParameters brightnessBlitParams =
        new RenderGraphUtils.BlitMaterialParameters()
        {
            source = luminanceExtractTexture,
            destination = luminanceBlurTexture,
            material = blurmaterial
        };

        renderGraph.AddBlitPass(
            brightnessBlitParams,
            "Brightness Blur Blit"
        );

        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Bloom Composite Pass", out CompositePassData passData))
        {
            //传入_BlitTexture的纹理
            passData.sourceTexture = cameraTexture;
            //传入辉度模糊后的纹理
            passData.otherTexture = luminanceBlurTexture;
            //输出到原始临时纹理
            passData.destinationTexture = orignalTempTexture;
            passData.material = compositeTextureMaterial;

            builder.UseTexture(passData.sourceTexture);
            builder.UseTexture(passData.otherTexture);

            builder.SetRenderAttachment(
                passData.destinationTexture, 
                0
            );

            builder.SetRenderFunc(
                (CompositePassData data, RasterGraphContext context) =>
                {
                    data.material.SetTexture(luminanceBlurTexID, data.otherTexture);
                        Blitter.BlitTexture(
                        context.cmd,
                        data.sourceTexture,
                        new Vector4(1, 1, 0, 0),
                        data.material,
                        0
                    );
                });
        }

        renderGraph.AddCopyPass(
            orignalTempTexture,
            cameraTexture,
            "Bloom Copy Pass"
        );

    }

}