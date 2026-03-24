using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemSelectionUIScript : MonoBehaviour
{

	/* object references */
	public Transform ArrowRight;
	public Transform ArrowLeft;
	public Transform ArrowTop;
	public Transform ArrowBottom;

	public Transform ItemInfoContainer;

	public TextMesh NameLabel;
	public TextMesh NameLabelShadow;

	public TextMesh LevelLabel;
	public TextMesh LevelLabelShadow;

	public SpriteRenderer Grid;
	public Sprite GridGreen;
	public Sprite GridRed;

	private Vector3 ExpandFromCenter(Vector3 localPos, float origWidth, float origHeight, float newWidth, float newHeight, float extraOffsetX = 0f, float extraOffsetZ = 0f)
	{
		Vector3 origCenter = new Vector3((origWidth - 1f) / 2f, 0, (origHeight - 1f) / 2f);
		Vector3 newCenter = new Vector3((newWidth - 1f) / 2f, 0, (newHeight - 1f) / 2f);

		Vector3 offset = localPos - origCenter;

		// Expand X offset if this element is meant to sit on the left or right edges
		if (Mathf.Abs(offset.x) > 0.01f)
		{
			offset.x += Mathf.Sign(offset.x) * ((newWidth - origWidth) / 2f + extraOffsetX);
		}

		// Expand Z offset if this element is meant to sit on the top or bottom edges
		if (Mathf.Abs(offset.z) > 0.01f)
		{
			offset.z += Mathf.Sign(offset.z) * ((newHeight - origHeight) / 2f + extraOffsetZ);
		}

		return newCenter + offset;
	}

	void Start()
	{
		BaseItemScript baseItem = this.GetComponentInParent<BaseItemScript>();
		float gw = baseItem.itemData.gridWidth;
		float gh = baseItem.itemData.gridHeight;

		this.transform.localScale = Vector3.one;
		this.transform.localPosition = new Vector3(baseItem.itemData.uiOffsetX, this.transform.localPosition.y, baseItem.itemData.uiOffsetZ);

		float ax = baseItem.itemData.arrowOffsetX;
		float az = baseItem.itemData.arrowOffsetZ;

		this.ArrowRight.localPosition = ExpandFromCenter(this.ArrowRight.localPosition, 1f, 1f, gw, gh, ax, az);
		this.ArrowLeft.localPosition = ExpandFromCenter(this.ArrowLeft.localPosition, 1f, 1f, gw, gh, ax, az);
		this.ArrowTop.localPosition = ExpandFromCenter(this.ArrowTop.localPosition, 1f, 1f, gw, gh, ax, az);
		this.ArrowBottom.localPosition = ExpandFromCenter(this.ArrowBottom.localPosition, 1f, 1f, gw, gh, ax, az);

		this.ItemInfoContainer.localPosition = ExpandFromCenter(this.ItemInfoContainer.localPosition, 1f, 1f, gw, gh);
		this.Grid.transform.localPosition = ExpandFromCenter(this.Grid.transform.localPosition, 1f, 1f, gw, gh);
		this.Grid.transform.localPosition += new Vector3(baseItem.itemData.gridOffsetX, 0, baseItem.itemData.gridOffsetZ);
		
		this.Grid.size = new Vector2(gw, gh);

		/* update item info details */
		this.NameLabel.text = this.NameLabelShadow.text = baseItem.itemData.name;
		this.RefreshLevel(baseItem.level);

		this.ShowGrid(false);
	}

	public void RefreshLevel(int level)
	{
		this.LevelLabel.text = this.LevelLabelShadow.text = "Level " + level;
	}

	public void ShowGrid(bool isTrue)
	{
		this.Grid.gameObject.SetActive(isTrue);
	}

	public void SetGridColor(Color color)
	{
		if (color == Color.green)
		{
			this.Grid.sprite = this.GridGreen;

		}
		else if (color == Color.red)
		{
			this.Grid.sprite = this.GridRed;
		}
	}
}
