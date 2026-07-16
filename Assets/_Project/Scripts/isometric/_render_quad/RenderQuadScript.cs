using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderQuadScript : MonoBehaviour
{

	/* object refs */
	public MeshFilter MeshFilter;
	public MeshRenderer MeshRenderer;

	/* private vars */
	private bool _wasModified = false;

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
		if (baseItem != null)
		{
			if (baseItem.state == Common.State.PREVIEW)
			{
				if (this.MeshRenderer != null && this.MeshRenderer.material != null && this.MeshRenderer.material.HasProperty("_Color"))
				{
					// Giữ nguyên alpha mờ cố định khi preview
					Color c = Color.white;
					c.a = 0.4f;
					this.MeshRenderer.material.color = c;
					_wasModified = true;
				}
			}
			else if (baseItem.isUnderConstruction)
			{
				if (this.MeshRenderer != null && this.MeshRenderer.material != null && this.MeshRenderer.material.HasProperty("_Color"))
				{
					float buildTime = baseItem.itemData != null && baseItem.itemData.configuration != null ? baseItem.itemData.configuration.buildTime : 0f;
					float progress = 0f;
					if (buildTime > 0f)
					{
						float remaining = baseItem.GetConstructionTimeRemaining();
						progress = Mathf.Clamp01(1f - (remaining / buildTime));
					}
					else
					{
						progress = 1f;
					}

					// Rõ dần tỉ lệ với tiến trình xây dựng (alpha từ 0.2f đến 1.0f)
					float alpha = Mathf.Lerp(0.2f, 1.0f, progress);
					Color c = Color.white;
					c.a = alpha;
					this.MeshRenderer.material.color = c;
					_wasModified = true;
				}
			}
			else if (_wasModified)
			{
				if (this.MeshRenderer != null && this.MeshRenderer.material != null && this.MeshRenderer.material.HasProperty("_Color"))
				{
					this.MeshRenderer.material.color = Color.white;
				}
				_wasModified = false;
			}
		}
	}
}
