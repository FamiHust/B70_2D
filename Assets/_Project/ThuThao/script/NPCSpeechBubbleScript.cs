using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpeechBubbleScript : MonoBehaviour
{
    public enum BubbleType
    {
        Complaint,  
        Ellipsis    
    }

    [Header("References")]
    public GameObject BubbleRoot;       
    public TMP_Text MessageText;            
    public Button EllipsisButton;       
    public GameObject EllipsisIcon;     
    public GameObject TextRoot;         

    [Header("New ChatBox Visuals (Sprite)")]
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer emojiRenderer;
    public TMP_Text textMeshPro;
    public Transform textHolder;
    public Vector3 holderOriginalScale;

    [HideInInspector] public BubbleType bubbleType;

    private Coroutine _autoHideCoroutine;
    private bool _isShown = false;

    public System.Action OnEllipsisTapped;

    void Awake()
    {
        
        if (BubbleRoot == null) BubbleRoot = this.gameObject;
        
        BubbleRoot.SetActive(false);
        
        if (EllipsisButton != null)
        {
            EllipsisButton.onClick.RemoveAllListeners();
            EllipsisButton.onClick.AddListener(_OnEllipsisTapped);
        }

        if (spriteRenderer != null && GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();

            float extraHeight = 3.5f;
            col.size = new Vector2(spriteRenderer.size.x, spriteRenderer.size.y + extraHeight);
            
            Vector2 baseOffset = spriteRenderer.transform != transform ? (Vector2)spriteRenderer.transform.localPosition : Vector2.zero;
            
            baseOffset.y -= extraHeight / 2f;
            
            col.offset = baseOffset;
        }
    }

    void OnMouseDown()
    {
        
        if (bubbleType == BubbleType.Ellipsis)
        {
            _OnEllipsisTapped();
        }
    }

    private float _currentAnimationScale = 1f;
    private Vector3 _initialScale = Vector3.one;

    void LateUpdate()
    {
        
        if (_isShown && Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;

            float scaleFactor = 1f;
            if (Camera.main.orthographic)
            {
                
                float referenceOrthoSize = 15f; 
                scaleFactor = Camera.main.orthographicSize / referenceOrthoSize;

                scaleFactor = Mathf.Clamp(scaleFactor, 0.2f, 3.0f);
            }

            if (BubbleRoot != null)
            {
                BubbleRoot.transform.localScale = _initialScale * (_currentAnimationScale * scaleFactor);
            }
        }
    }

    public void ShowComplaint(string message, float duration = 5f)
    {
        bubbleType = BubbleType.Complaint;
        _SetupVisuals(message, isEllipsis: false);
        _isShown = true;

        if (_autoHideCoroutine != null) StopCoroutine(_autoHideCoroutine);
        _autoHideCoroutine = StartCoroutine(_AutoHide(duration));
    }

    public void ShowEllipsis()
    {
        bubbleType = BubbleType.Ellipsis;
        _SetupVisuals("...", isEllipsis: true);
        _isShown = true;

        if (_autoHideCoroutine != null)
        {
            StopCoroutine(_autoHideCoroutine);
            _autoHideCoroutine = null;
        }
    }

    public void Hide()
    {
        if (_autoHideCoroutine != null)
        {
            StopCoroutine(_autoHideCoroutine);
            _autoHideCoroutine = null;
        }
        _isShown = false;
        if (BubbleRoot != null) BubbleRoot.SetActive(false);
    }

    private void _SetupVisuals(string message, bool isEllipsis)
    {
        if (BubbleRoot != null) BubbleRoot.SetActive(true);

        if (isEllipsis)
        {
            
            if (EllipsisButton != null) EllipsisButton.gameObject.SetActive(true);
            if (EllipsisIcon != null)   EllipsisIcon.SetActive(false);
            if (MessageText != null)    MessageText.gameObject.SetActive(false);
            if (TextRoot != null)       TextRoot.SetActive(false); 

            if (emojiRenderer != null) emojiRenderer.gameObject.SetActive(false); 

            if (textMeshPro == null && textHolder != null)
            {
                _CreateDynamicText();
            }

            if (textMeshPro != null)
            {
                textMeshPro.gameObject.SetActive(true);
                textMeshPro.text = "...";
                textMeshPro.enableAutoSizing = false;
                textMeshPro.fontSize = 8.0f; 
            }
        }
        else
        {
            
            if (MessageText != null)
            {
                MessageText.gameObject.SetActive(true);
                MessageText.text = message;
            }
            if (TextRoot != null)       TextRoot.SetActive(true); 
            if (EllipsisButton != null) EllipsisButton.gameObject.SetActive(false);
            if (EllipsisIcon != null)   EllipsisIcon.SetActive(false);

            if (emojiRenderer != null) emojiRenderer.gameObject.SetActive(false);

            if (textMeshPro == null && textHolder != null)
            {
                _CreateDynamicText();
            }

            if (textMeshPro != null)
            {
                textMeshPro.gameObject.SetActive(true);
                textMeshPro.text = message;
                textMeshPro.enableAutoSizing = true;
                textMeshPro.fontSizeMin = 1.5f;
                textMeshPro.fontSizeMax = 10.0f;
            }
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.sortingOrder = 32000; 
        }

        if (BubbleRoot != null)
        {
            _initialScale = BubbleRoot.transform.localScale;
            if (_initialScale == Vector3.zero) _initialScale = Vector3.one;
            _currentAnimationScale = 0f;
            StartCoroutine(_ScaleIn());
        }
    }

    private void _CreateDynamicText()
    {
        
        GameObject txtObj = new GameObject("DynamicText");

        if (spriteRenderer != null)
        {
            txtObj.transform.SetParent(spriteRenderer.transform, false);
        }
        else if (textHolder != null)
        {
            txtObj.transform.SetParent(textHolder, false);
        }
        
        txtObj.transform.localPosition = new Vector3(0, 0, -0.1f); 
        txtObj.transform.localScale = Vector3.one;

        var tmp = txtObj.AddComponent<TextMeshPro>();
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 1.5f;
        tmp.fontSizeMax = 10.0f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        tmp.overflowMode = TextOverflowModes.Overflow; 
        
        MeshRenderer mr = txtObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingOrder = 32001; 
            if (spriteRenderer != null)
            {
                mr.sortingLayerID = spriteRenderer.sortingLayerID;
            }
        }
        
        textMeshPro = tmp;

        RectTransform rt = textMeshPro.GetComponent<RectTransform>();
        if (spriteRenderer != null)
        {
            rt.sizeDelta = new Vector2(spriteRenderer.size.x * 0.9f, spriteRenderer.size.y * 0.9f);
        }
        else
        {
            rt.sizeDelta = new Vector2(3f, 1.5f);
        }
    }

    private IEnumerator _ScaleIn()
    {
        float t = 0f;
        float duration = 0.2f;
        while (t < duration)
        {
            t += Time.deltaTime;
            _currentAnimationScale = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        _currentAnimationScale = 1f;
    }

    private IEnumerator _AutoHide(float duration)
    {
        yield return new WaitForSeconds(duration);
        Hide();
        _autoHideCoroutine = null;
    }

    private void _OnEllipsisTapped()
    {
        if (OnEllipsisTapped != null)
            OnEllipsisTapped.Invoke();
    }
}
