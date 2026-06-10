using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionWindowScript : WindowScript
{
    [Header("References")]
    public Transform cardZone;
    public GameObject teacherCardPrefab;
    public ScrollRect scrollView;

    [Header("Preview Settings")]
    public TeacherCardCtrl previewCard;
    public Button confirmButton;
    public Button switchButton;
    public Animator anim;

    private List<TeacherCardCtrl> _instantiatedCards = new List<TeacherCardCtrl>();
    private bool _isAssignMode = false;
    private bool _isSwitchMode = false;
    private BaseItemScript _targetBuilding;
    private TeacherData _selectedData;

    private void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmSelection);
        }
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(OnConfirmSwitch);
        }
    }

    private void FindPreviewCardIfNull()
    {
        if (previewCard == null)
        {
            foreach (var card in GetComponentsInChildren<TeacherCardCtrl>(true))
            {
                if (cardZone == null || !card.transform.IsChildOf(cardZone))
                {
                    previewCard = card;
                    Debug.LogWarning("[CollectionWindow] previewCard was null, auto-found: " + card.name);
                    break;
                }
            }
        }
    }

    private bool IsTeacherAssignedToAnyBuilding(TeacherData teacher)
    {
        if (teacher == null) return false;
        if (SceneManager.instance == null) return false;

        var items = SceneManager.instance.GetItemInstances();
        if (items != null)
        {
            foreach (var item in items.Values)
            {
                if (item != null && item.assignedTeacher != null && item.assignedTeacher.id == teacher.id)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Setup(List<TeacherData> playerInventory, bool isAssignMode = false, BaseItemScript targetBuilding = null, bool isSwitchMode = false)
    {
        FindPreviewCardIfNull();
        this._isAssignMode = isAssignMode;
        this._isSwitchMode = isSwitchMode;
        this._targetBuilding = targetBuilding;
        this._selectedData = null;

        if (previewCard != null)
        {
            previewCard.SetData(null);
            previewCard.gameObject.SetActive(false);
            previewCard.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }

        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(isAssignMode && !isSwitchMode);
            confirmButton.interactable = false;
        }

        if (switchButton != null)
        {
            switchButton.gameObject.SetActive(isSwitchMode);
            switchButton.interactable = false;
        }

        TeacherCardCtrl[] existingCards = cardZone.GetComponentsInChildren<TeacherCardCtrl>(true);

        if (scrollView != null)
        {
            scrollView.verticalNormalizedPosition = 1.0f; // Scroll to top
        }

        int inventoryCount = playerInventory != null ? playerInventory.Count : 0;

        for (int i = 0; i < existingCards.Length; i++)
        {
            if (i < inventoryCount)
            {
                TeacherData teacher = playerInventory[i];
                existingCards[i].SetData(teacher, OnCardClicked);

                if (IsTeacherAssignedToAnyBuilding(teacher))
                {
                    if (existingCards[i].nameText != null)
                    {
                        existingCards[i].nameText.text = teacher.teacherName + " (Đã thuê)";
                    }
                }
            }
            else
            {
                existingCards[i].SetData(null, null); // Empty slot
            }
        }
    }

    private void OnCardClicked(TeacherCardCtrl clickedCard)
    {
        FindPreviewCardIfNull();
        _selectedData = clickedCard.currentData;
        Debug.Log($"[CollectionWindow] OnCardClicked: card={clickedCard.name}, data={(_selectedData != null ? _selectedData.teacherName : "null")}, previewCard={(previewCard != null ? previewCard.name : "null")}");

        // Show preview
        if (previewCard != null)
        {
            previewCard.gameObject.SetActive(true);
            previewCard.SetData(_selectedData);

            if (IsTeacherAssignedToAnyBuilding(_selectedData))
            {
                if (previewCard.nameText != null)
                {
                    previewCard.nameText.text = _selectedData.teacherName + " (Đã thuê)";
                }
            }
        }

        // Enable confirm button if in assign mode and teacher is not assigned to any building
        if (_isAssignMode && !_isSwitchMode && confirmButton != null)
        {
            if (IsTeacherAssignedToAnyBuilding(_selectedData))
            {
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                confirmButton.gameObject.SetActive(true);
                confirmButton.interactable = true;
            }
        }

        // Enable switch button if in switch mode and teacher is not assigned to any building
        if (_isSwitchMode && switchButton != null)
        {
            if (IsTeacherAssignedToAnyBuilding(_selectedData))
            {
                switchButton.gameObject.SetActive(false);
            }
            else
            {
                switchButton.gameObject.SetActive(true);
                switchButton.interactable = true;
            }
        }
    }

    private void OnConfirmSelection()
    {
        if (_isAssignMode && _targetBuilding != null && _selectedData != null)
        {
            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(false);
            }

            // Assign the teacher to the building
            _targetBuilding.assignedTeacher = _selectedData;
            
            // Save assignment to PlayerPrefs
            PlayerPrefs.SetInt("BuildingTeacher_" + _targetBuilding.instanceId, _selectedData.id);
            PlayerPrefs.Save();

            // Refresh InfoWindow if open to display the newly hired teacher and update stats
            if (InfoWindowScript.instance != null)
            {
                InfoWindowScript.instance.RenderInfo();
            }
            
            // Close the window after assigning
            Close();
        }
    }

    private void OnConfirmSwitch()
    {
        if (_isSwitchMode && _targetBuilding != null && _selectedData != null)
        {
            if (switchButton != null)
            {
                switchButton.gameObject.SetActive(false);
            }

            // Assign the teacher to the building
            _targetBuilding.assignedTeacher = _selectedData;
            
            // Save assignment to PlayerPrefs
            PlayerPrefs.SetInt("BuildingTeacher_" + _targetBuilding.instanceId, _selectedData.id);
            PlayerPrefs.Save();

            // Refresh InfoWindow if open to display the newly hired teacher and update stats
            if (InfoWindowScript.instance != null)
            {
                InfoWindowScript.instance.RenderInfo();
            }
            
            // Close the window after assigning
            Close();
        }
    }

    public void HideWindow()
    {
        if (anim != null) anim.Play("Hide");
    }

    public void ShowWindow()
    {
        if (anim != null) anim.Play("Show");
    }

    public void ToggleGameObject(GameObject target)
    {
        if (target != null)
        {
            target.SetActive(!target.activeSelf);
        }
    }
}
