using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "BloomRenderFeature", menuName = "Scriptable Objects/BloomRenderFeature")]
public class BloomRenderFeature : ScriptableRendererFeature
{
	public Material luminanceExtractMaterial = null;
	public Material blurMaterial = null;
	public Material compositeMaterial = null;

	private BloomRenderPass bloomRenderPass = null;

	public override void Create()
	{
		bloomRenderPass = new BloomRenderPass(luminanceExtractMaterial, blurMaterial, compositeMaterial);
		//bloomRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}	
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(bloomRenderPass);
	}
	
}
