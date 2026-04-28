using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemProgressUIScript : MonoBehaviour
{

	/* object references */
	public Transform ProgressContainer;

	public TextMesh TimerLabel;
	public TextMesh TimerLabelShadow;

	public SpriteRenderer ProgressFiller;

	/* private vars */
	private BaseItemScript _baseItem;
	private float _buildTime;
	private float _buildStartTime;
	private float _fillerFullLength;

	void Start()
	{
		this._baseItem = this.GetComponentInParent<BaseItemScript>();
		this._fillerFullLength = this.ProgressFiller.size.x;

		float gw = this._baseItem.itemData.gridWidth;
		float gh = this._baseItem.itemData.gridHeight;
		this.transform.localPosition = new Vector3((gw - 1f) / 2f, this.transform.localPosition.y, (gh - 1f) / 2f);

		this.Init();
	}

	void Update()
	{
		this.UpdateProgress();
	}

	public void Init()
	{
		this._buildTime = this._baseItem.itemData.configuration.buildTime;
		this._buildStartTime = Time.time;
	}

	private Vector2 _tempSize;
	public void UpdateProgress()
	{
		if (this._buildTime <= 0)
		{
			this.OnFinishBuild();
			return;
		}

		float elapsedTime = Time.time - this._buildStartTime;
		float progress = elapsedTime / this._buildTime;

		float oldWidth = this.ProgressFiller.size.x;
		_tempSize.x = progress * this._fillerFullLength;
		_tempSize.y = this.ProgressFiller.size.y;
		
		// Adjust position to keep left edge fixed while growing to the right
		float positionOffset = (_tempSize.x - oldWidth) / 2f;
		this.ProgressFiller.transform.localPosition += new Vector3(positionOffset, 0, 0);
		
		this.ProgressFiller.size = _tempSize;

		int timeToFinish = (int)(_buildTime - elapsedTime);
		TimerLabel.text = timeToFinish.ToString() + "s";
		TimerLabelShadow.text = timeToFinish.ToString() + "s";

		if (progress >= 1)
		{
			this.OnFinishBuild();
		}
	}

	public void SetFillerColor(Color color)
	{

	}

	public void OnFinishBuild()
	{
		this._baseItem.UI.ShowProgressUI(false);
	}

	public float GetProgress()
	{
		if (this._buildTime <= 0) return 1f;
		float elapsedTime = Time.time - this._buildStartTime;
		float progress = elapsedTime / this._buildTime;
		return Mathf.Clamp01(progress);
	}
}
