@script ExecuteInEditMode

class EdgeDetectEffectNormals extends ImageEffectBase
{	
	function Start() {
		GetComponent.<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}
		
	function OnRenderImage (source : RenderTexture, destination : RenderTexture)
	{
		ImageEffects.BlitWithMaterial (material, source, destination);
	}
}
