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

    public float tweenDuration = 0.75f;

    private void UpdateComponents()
    {
        if (ValueLabel == null || Filler == null) return;

        if (isPercent)
        {
            if (hasMaxValue && maxValue > 0)
            {
                int percent = Mathf.RoundToInt(value / maxValue * 100f);
                ValueLabel.text = percent.ToString() + "%";
            }
            else
            {
                ValueLabel.text = ((int)value).ToString() + "%";
            }
        }
        else if (showAsCurrentMax && hasMaxValue)
        {
            ValueLabel.text = ((int)value).ToString() + "/" + ((int)maxValue).ToString();
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
            StopAllCoroutines(); // Tránh xung đột nếu có tween cũ đang chạy
            StartCoroutine(_TweenValueChange(changedValue));
        }
        else
        {
            value = changedValue;
        }
    }

    private IEnumerator _TweenValueChange(float changedValue)
    {
        float startValue = value;
        float elapsed = 0f;

        while (elapsed < tweenDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / tweenDuration;
            
            // SmoothStep: mượt mà hơn ở điểm đầu và cuối
            t = t * t * (3f - 2f * t);
            
            value = Mathf.Lerp(startValue, changedValue, t);
            yield return null;
        }

        value = changedValue;
    }
}
