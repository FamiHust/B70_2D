using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderQuadScript : MonoBehaviour
{

	/* object refs */
	public MeshFilter MeshFilter;
	public MeshRenderer MeshRenderer;

	/* private vars */

	public void SetData(SpriteCollection.TextureData textureData, int layer)
	{
		this.MeshRenderer.material = Sprites.GetTextureMaterial(textureData.texture, textureData.parent.renderingLayer, textureData.parent.renderingOrder);
		this.MeshRenderer.sortingOrder = textureData.parent.renderingOrder;
	}

	public void SetAlpha(float alpha)
	{
		if (this.MeshRenderer.material.HasProperty("_Color"))
		{
			Color c = this.MeshRenderer.material.color;
			c.a = alpha;
			this.MeshRenderer.material.color = c;
		}
	}

	private void Update()
	{
		BaseItemScript baseItem = GetComponentInParent<BaseItemScript>();
		if (baseItem != null && baseItem.state == Common.State.PREVIEW)
		{
			if (this.MeshRenderer != null && this.MeshRenderer.material != null && this.MeshRenderer.material.HasProperty("_Color"))
			{
				float lerp = Mathf.PingPong(Time.time * 4f, 1f);
				Color baseColor = Color.white;
				Color targetColor = Color.red;
				
				Color c = Color.Lerp(baseColor, targetColor, lerp);
				c.a = 0.5f;
				this.MeshRenderer.material.color = c;
			}
		}
	}
}
