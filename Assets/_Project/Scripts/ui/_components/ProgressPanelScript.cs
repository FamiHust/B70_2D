using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPanelScript : MonoBehaviour
{
    /* object references */
    public Image Filler;   // đổi từ RectTransform → Image
    public Text ValueLabel;

    /* public variables */
    public bool hasMaxValue;
    public bool isPercent;
    public bool showAsCurrentMax;  // Hiển thị dạng "current/max" (e.g. 5/10)

    private float _maxValue;
    public float maxValue
    {
        get { return _maxValue; }
        set
        {
            _maxValue = value;
            UpdateComponents();
        }
    }

    private float _value;
    public float value
    {
        get { return _value; }
        set
        {
            _value = value;
            UpdateComponents();
        }
    }

    public void UpdateComponents()
{
    if (showAsCurrentMax && hasMaxValue)
    {
        // Hiển thị dạng "current/max" (e.g. 5/10)
        ValueLabel.text = ((int)value) + "/" + ((int)maxValue);
    }
    else if (isPercent)
    {
        ValueLabel.text = ((int)value) + "%";
    }
    else
    {
        ValueLabel.text = ((int)value).ToString();
    }

    if (hasMaxValue && maxValue > 0)
    {
        float progress = value / maxValue;
        SetProgress(progress);
    }
}

    public void SetProgress(float progress)
    {
        Filler.fillAmount = Mathf.Clamp01(progress);
    }

    public void TweenValueChange(float changedValue)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(_TweenValueChange(changedValue));
        }
        else
        {
            value = changedValue;
        }
    }

    private IEnumerator _TweenValueChange(float changedValue)
    {
        int oldValue = (int)value;

        if (changedValue > oldValue)
        {
            for (int i = oldValue; i < (int)changedValue; i++)
            {
                yield return null;
                value++;
            }
        }
        else
        {
            for (int i = oldValue; i > (int)changedValue; i--)
            {
                yield return null;
                value--;
            }
        }

        value = changedValue;
    }
}