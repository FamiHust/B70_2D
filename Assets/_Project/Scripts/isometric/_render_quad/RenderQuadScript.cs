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

}
