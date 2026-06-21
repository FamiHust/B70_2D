using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// A custom layout component that arranges child UI elements in a curved arc.
/// Works both standalone or alongside a standard HorizontalLayoutGroup.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class CurvedHorizontalLayoutGroup : MonoBehaviour
{
    [Header("Curve Settings")]
    [Tooltip("Radius of the curvature. Positive values curve downward (smile), negative values curve upward (rainbow).")]
    public float radius = 500f;

    [Tooltip("Constant vertical offset applied to all children.")]
    public float yOffset = 0f;

    [Tooltip("If true, rotates children to align with the curve.")]
    public bool rotateChildren = true;

    [Tooltip("Reverses the rotation angle of the children.")]
    public bool reverseRotation = false;

    [Header("Standalone Spacing Settings (When layout group is disabled)")]
    [Tooltip("If true, children are evenly spaced across the container width. If false, specified spacing is used.")]
    public bool useAutoSpacing = true;

    [Tooltip("Horizontal spacing between children (only used if useAutoSpacing is false).")]
    public float spacing = 100f;

    [Tooltip("Left and right padding from the container edges.")]
    public float paddingLeft = 0f;
    public float paddingRight = 0f;

    private RectTransform _rectTransform;

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private void LateUpdate()
    {
        HorizontalLayoutGroup hLayout = GetComponent<HorizontalLayoutGroup>();
        bool hasActiveLayout = hLayout != null && hLayout.enabled;

        List<RectTransform> children = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            if (child != null && child.gameObject.activeSelf)
            {
                children.Add(child);
            }
        }

        int count = children.Count;
        if (count == 0) return;

        float absRadius = Mathf.Max(0.1f, Mathf.Abs(radius));

        if (hasActiveLayout)
        {
            // If HorizontalLayoutGroup is active, it already positioned children horizontally.
            // We run in LateUpdate to override Y positions and apply rotation after layout pass.
            for (int i = 0; i < count; i++)
            {
                RectTransform child = children[i];
                float x = child.localPosition.x;
                float clampedX = Mathf.Clamp(x, -absRadius, absRadius);
                float y = Mathf.Sqrt(absRadius * absRadius - clampedX * clampedX) - absRadius;

                if (radius < 0)
                {
                    y = -y;
                }

                child.localPosition = new Vector3(x, y + yOffset, child.localPosition.z);

                if (rotateChildren)
                {
                    float angleRad = Mathf.Asin(clampedX / absRadius);
                    float angleDeg = angleRad * Mathf.Rad2Deg;
                    float rotAngle = radius < 0 ? angleDeg : -angleDeg;

                    if (reverseRotation)
                    {
                        rotAngle = -rotAngle;
                    }

                    child.localRotation = Quaternion.Euler(0f, 0f, rotAngle);
                }
                else
                {
                    child.localRotation = Quaternion.identity;
                }
            }
        }
        else
        {
            // If no layout group is active, we position them horizontally ourselves.
            float width = rectTransform.rect.width;
            float usableWidth = width - paddingLeft - paddingRight;

            float startX = 0f;
            float step = 0f;

            if (useAutoSpacing)
            {
                if (count > 1)
                {
                    startX = -usableWidth / 2f;
                    step = usableWidth / (count - 1);
                }
                else
                {
                    startX = 0f;
                    step = 0f;
                }
            }
            else
            {
                float totalSpacingWidth = (count - 1) * spacing;
                startX = -totalSpacingWidth / 2f;
                step = spacing;
            }

            for (int i = 0; i < count; i++)
            {
                RectTransform child = children[i];
                child.anchorMin = new Vector2(0.5f, 0.5f);
                child.anchorMax = new Vector2(0.5f, 0.5f);
                child.pivot = new Vector2(0.5f, 0.5f);

                float x = startX + i * step;
                float clampedX = Mathf.Clamp(x, -absRadius, absRadius);
                float y = Mathf.Sqrt(absRadius * absRadius - clampedX * clampedX) - absRadius;

                if (radius < 0)
                {
                    y = -y;
                }

                child.localPosition = new Vector3(x, y + yOffset, child.localPosition.z);

                if (rotateChildren)
                {
                    float angleRad = Mathf.Asin(clampedX / absRadius);
                    float angleDeg = angleRad * Mathf.Rad2Deg;
                    float rotAngle = radius < 0 ? angleDeg : -angleDeg;

                    if (reverseRotation)
                    {
                        rotAngle = -rotAngle;
                    }

                    child.localRotation = Quaternion.Euler(0f, 0f, rotAngle);
                }
                else
                {
                    child.localRotation = Quaternion.identity;
                }
            }
        }
    }
}
