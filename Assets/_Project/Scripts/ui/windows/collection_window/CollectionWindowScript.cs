using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionWindowScript : WindowScript
{
    [Header("References")]
    public Transform cardZone;
    public GameObject teacherCardPrefab;
    public ScrollRect scrollView;
    public Text teacherPriceText;

    [Header("Preview Settings")]
    public TeacherCardCtrl previewCard;
    public Button confirmButton;
    public Button switchButton;
    public Button hiringButton;
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
            confirmButton.onClick.AddListener(OnHiringSelection);
        }
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(OnConfirmSwitch);
        }
        if (hiringButton != null)
        {
            hiringButton.onClick.AddListener(OnConfirmSelection);
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

    private bool IsTeacherHired(TeacherData teacher)
    {
        if (teacher == null) return false;
        if (IsTeacherAssignedToAnyBuilding(teacher)) return true;
        return PlayerPrefs.GetInt("TeacherHired_" + teacher.id, 0) == 1;
    }

    private void OnCardClickedForData(TeacherData data)
    {
        if (data == null) return;
        TeacherCardCtrl[] existingCards = cardZone.GetComponentsInChildren<TeacherCardCtrl>(true);
        foreach (var card in existingCards)
        {
            if (card != null && card.currentData != null && card.currentData.id == data.id)
            {
                OnCardClicked(card);
                break;
            }
        }
    }

    private void UpdateButtonStates()
    {
        if (_selectedData == null)
        {
            if (confirmButton != null) confirmButton.gameObject.SetActive(false);
            if (switchButton != null) switchButton.gameObject.SetActive(false);
            if (hiringButton != null) hiringButton.gameObject.SetActive(false);
            return;
        }

        bool isHired = IsTeacherHired(_selectedData);
        bool isAssigned = IsTeacherAssignedToAnyBuilding(_selectedData);

        if (!isHired)
        {
            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(true);
                confirmButton.interactable = true;
            }
            if (hiringButton != null)
            {
                hiringButton.gameObject.SetActive(false);
            }
            if (switchButton != null)
            {
                switchButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(false);
            }

            if (_targetBuilding != null && !isAssigned)
            {
                if (_isSwitchMode)
                {
                    if (switchButton != null)
                    {
                        switchButton.gameObject.SetActive(true);
                        switchButton.interactable = true;
                    }
                    if (hiringButton != null)
                    {
                        hiringButton.gameObject.SetActive(false);
                    }
                }
                else if (_isAssignMode)
                {
                    if (hiringButton != null)
                    {
                        hiringButton.gameObject.SetActive(true);
                        hiringButton.interactable = true;
                    }
                    if (switchButton != null)
                    {
                        switchButton.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (switchButton != null) switchButton.gameObject.SetActive(false);
                    if (hiringButton != null) hiringButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (switchButton != null) switchButton.gameObject.SetActive(false);
                if (hiringButton != null) hiringButton.gameObject.SetActive(false);
            }
        }
    }

    public void Setup(List<TeacherData> playerInventory, bool isAssignMode = false, BaseItemScript targetBuilding = null, bool isSwitchMode = false)
    {
        FindPreviewCardIfNull();
        this._isAssignMode = isAssignMode;
        this._isSwitchMode = isSwitchMode;
        this._targetBuilding = targetBuilding;
        this._selectedData = null;

        if (teacherPriceText != null)
        {
            teacherPriceText.text = "";
        }

        if (previewCard != null)
        {
            previewCard.SetData(null);
            previewCard.gameObject.SetActive(false);
            previewCard.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }

        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
        }

        if (switchButton != null)
        {
            switchButton.gameObject.SetActive(false);
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
                    if (existingCards[i].statusText != null)
                    {
                        existingCards[i].statusText.text = "Đang dạy";
                    }
                }
                else if (IsTeacherHired(teacher))
                {
                    if (existingCards[i].statusText != null)
                    {
                        existingCards[i].statusText.text = "Đã thuê";
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
                if (previewCard.statusText != null)
                {
                    previewCard.statusText.text = "Đang dạy";
                }
            }
            else if (IsTeacherHired(_selectedData))
            {
                if (previewCard.statusText != null)
                {
                    previewCard.statusText.text = "Đã thuê";
                }
            }
        }

        UpdateButtonStates();

        if (teacherPriceText != null)
        {
            if (_selectedData != null)
            {
                if (IsTeacherHired(_selectedData))
                {
                    teacherPriceText.text = "Công tác";
                }
                else
                {
                    teacherPriceText.text = "Thuê GV: " + _selectedData.hirePrice.ToString();
                }
            }
            else
            {
                teacherPriceText.text = "";
            }
        }
    }

    private void OnHiringSelection()
    {
        if (_selectedData == null) return;

        bool isHired = IsTeacherHired(_selectedData);
        if (!isHired)
        {
            // Try to hire
            int cost = _selectedData.hirePrice;
            if (SceneManager.instance != null && SceneManager.instance.ConsumeResource("gold", cost))
            {
                // Mark teacher as hired
                PlayerPrefs.SetInt("TeacherHired_" + _selectedData.id, 1);
                PlayerPrefs.Save();

                // Refresh UI
                Setup(UIManager.instance.playerTeachers, _isAssignMode, _targetBuilding, _isSwitchMode);
                OnCardClickedForData(_selectedData);
            }
            else
            {
                // Show insufficient gold warning
                if (SceneManager.instance != null && UIManager.instance != null)
                {
                    int currentAmount = SceneManager.instance.numberOfGoldInStorage;
                    int missingAmount = cost - currentAmount;
                    WarningWindow warningWindow = UIManager.instance.ShowWarningWindow();
                    if (warningWindow != null)
                    {
                        warningWindow.SetupGoldWarning(missingAmount, currentAmount);
                    }
                }
                Debug.Log("Not enough gold to hire teacher!");
            }
        }
    }

    private void OnConfirmSelection()
    {
        if (_selectedData == null) return;

        bool isHired = IsTeacherHired(_selectedData);
        if (!isHired) return;

        if (_isAssignMode && _targetBuilding != null)
        {
            if (hiringButton != null)
            {
                hiringButton.gameObject.SetActive(false);
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
