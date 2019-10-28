using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PostEffect.MobileBloom.Bloom
{
	public class MobileBloom : MonoBehaviour{
		
		[FormerlySerializedAs("BloomAmount")] [Range(0, 5)]
		public float bloomAmount = 1f;
		[FormerlySerializedAs("BlurAmount")] [Range(0, 5)]
		public float blurAmount = 2f;
		[FormerlySerializedAs("FadeAmount")] [Range(0, 1)]
		public float fadeAmount = 0.2f;

		private static readonly int ScrWidth=Screen.width/4;
		private static readonly int ScrHeight=Screen.height/4;
		private static readonly int BlAmountString = Shader.PropertyToID("_BloomAmount");
		private static readonly int BlurAmountString = Shader.PropertyToID("_BlurAmount");
		private static readonly int FadeAmountString = Shader.PropertyToID("_FadeAmount");
		[FormerlySerializedAs("BloomTexString")] [SerializeField]
		private int bloomTexString = Shader.PropertyToID("_BloomTex");
		public Material material=null;

		private void  OnRenderImage (RenderTexture source ,   RenderTexture destination)
		{
			material.SetFloat(BlurAmountString, blurAmount/2.0f);
			material.SetFloat(FadeAmountString, fadeAmount);
			var buffer = RenderTexture.GetTemporary(ScrWidth, ScrHeight, 0,source.format);
			Graphics.Blit(source, buffer, material,0);

			var temp = RenderTexture.GetTemporary(ScrWidth/2, ScrHeight/2, 0, source.format);
			Graphics.Blit(buffer, temp, material, 1);
			RenderTexture.ReleaseTemporary(buffer);

			var temp2 = RenderTexture.GetTemporary(ScrWidth, ScrHeight, 0, source.format);
			Graphics.Blit(temp, temp2, material, 1);
			RenderTexture.ReleaseTemporary(temp);

			material.SetFloat(BlAmountString, bloomAmount);
			material.SetTexture(bloomTexString, temp2);
			Graphics.Blit(source, destination, material,2);
			RenderTexture.ReleaseTemporary(temp2);
		}

		public void Blor(Slider a)
		{
			blurAmount = a.value;
		}
		public void Blum(Slider a)
		{
			bloomAmount = a.value;
		}
	}
}
