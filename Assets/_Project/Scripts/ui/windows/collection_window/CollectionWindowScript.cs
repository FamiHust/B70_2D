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

    private List<TeacherCardCtrl> _instantiatedCards = new List<TeacherCardCtrl>();
    private bool _isAssignMode = false;
    private BaseItemScript _targetBuilding;
    private TeacherData _selectedData;

    private void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmSelection);
        }
    }

    public void Setup(List<TeacherData> playerInventory, bool isAssignMode = false, BaseItemScript targetBuilding = null)
    {
        this._isAssignMode = isAssignMode;
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
            confirmButton.gameObject.SetActive(isAssignMode);
            confirmButton.interactable = false;
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
                // Always pass callback to show preview
                existingCards[i].SetData(playerInventory[i], OnCardClicked);
            }
            else
            {
                existingCards[i].SetData(null, null); // Empty slot
            }
        }
    }

    private void OnCardClicked(TeacherCardCtrl clickedCard)
    {
        _selectedData = clickedCard.currentData;

        // Show preview
        if (previewCard != null)
        {
            previewCard.gameObject.SetActive(true);
            previewCard.SetData(_selectedData);
        }

        // Enable confirm button if in assign mode
        if (_isAssignMode && confirmButton != null)
        {
            confirmButton.interactable = true;
        }
    }

    private void OnConfirmSelection()
    {
        if (_isAssignMode && _targetBuilding != null && _selectedData != null)
        {
            // Assign the teacher to the building
            _targetBuilding.assignedTeacher = _selectedData;
            
            // Save assignment to PlayerPrefs
            PlayerPrefs.SetInt("BuildingTeacher_" + _targetBuilding.instanceId, _selectedData.id);
            PlayerPrefs.Save();
            
            // Close the window after assigning
            Close();
        }
    }
}
