using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
[ExecuteAlways]
public class CurvedText : BaseMeshEffect
{
    public enum CurveMode
    {
        DeformCharacters,
        RotateCharacters
    }

    [Tooltip("How the text is bent. Deform characters will stretch them along the curve. Rotate characters will keep their shape but rotate them.")]
    public CurveMode curveMode = CurveMode.DeformCharacters;

    [Tooltip("Radius of the curve. Higher values mean less curve. Negative values curve upwards (like a smile).")]
    public float radius = 100f;

    [Tooltip("Scale factor for Y axis. Adjusts how tall the text is relative to the curve. (Only used in Deform mode)")]
    public float scaleFactor = 1f;

    [Tooltip("If true, the curve is centered at the RectTransform's center. If false, it's centered at the text's bounding box center.")]
    public bool centerOnRectTransform = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        Graphic graphic = GetComponent<Graphic>();
        if (graphic != null)
            graphic.SetAllDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (Mathf.Abs(radius) < 0.001f)
            radius = 0.001f * Mathf.Sign(radius == 0 ? 1 : radius);

        Graphic graphic = GetComponent<Graphic>();
        if (graphic != null)
            graphic.SetAllDirty();
    }
#endif

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        if (vertices.Count == 0)
            return;

        float xOffset = 0f;

        if (!centerOnRectTransform)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 pos = vertices[i].position;
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
            }
            xOffset = (minX + maxX) / 2f;
        }
        else
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            xOffset = (0.5f - rectTransform.pivot.x) * rectTransform.rect.width;
        }

        if (curveMode == CurveMode.DeformCharacters)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                UIVertex vertex = vertices[i];

                float centeredX = vertex.position.x - xOffset;
                float angle = centeredX / radius;

                float currentRadius = radius + vertex.position.y * scaleFactor;

                float x = Mathf.Sin(angle) * currentRadius + xOffset;
                float y = Mathf.Cos(angle) * currentRadius - radius;

                vertex.position = new Vector3(x, y, vertex.position.z);
                vertices[i] = vertex;
            }
        }
        else if (curveMode == CurveMode.RotateCharacters)
        {
            // Characters are made of 6 vertices (2 triangles)
            for (int i = 0; i < vertices.Count; i += 6)
            {
                int endIdx = Mathf.Min(i + 6, vertices.Count);
                
                float charMinX = float.MaxValue;
                float charMaxX = float.MinValue;

                for (int j = i; j < endIdx; j++)
                {
                    float xPos = vertices[j].position.x;
                    if (xPos < charMinX) charMinX = xPos;
                    if (xPos > charMaxX) charMaxX = xPos;
                }

                float charCenterX = (charMinX + charMaxX) / 2f;
                float centeredX = charCenterX - xOffset;
                float angle = centeredX / radius;

                float charCurvedX = Mathf.Sin(angle) * radius + xOffset;
                float charCurvedY = Mathf.Cos(angle) * radius - radius;

                for (int j = i; j < endIdx; j++)
                {
                    UIVertex vertex = vertices[j];

                    float offsetX = vertex.position.x - charCenterX;
                    float offsetY = vertex.position.y;

                    float rotX = offsetX * Mathf.Cos(angle) + offsetY * Mathf.Sin(angle);
                    float rotY = -offsetX * Mathf.Sin(angle) + offsetY * Mathf.Cos(angle);

                    vertex.position = new Vector3(charCurvedX + rotX, charCurvedY + rotY, vertex.position.z);
                    vertices[j] = vertex;
                }
            }
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }
}
