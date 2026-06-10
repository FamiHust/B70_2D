using UnityEngine;
using UnityEngine.UI;
using System;

public class TeacherCardCtrl : MonoBehaviour
{
    [Header("UI References")]
    public GameObject lockImage;
    public Image avatarImage;
    public Image sexIconImage;
    public Text nameText;
    public Text levelText;
    public Text seniorityText;
    public Text buffGoldText;
    public Text buffEducationText;
    public Text buffHappyText;
    public Text descGoldText;
    public Text descEducationText;
    public Text descHappyText;
    public Button cardButton;

    public TeacherData currentData { get; private set; }

    private Action<TeacherCardCtrl> _onClickCallback;

    private void Awake()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnClickCard);
        }
    }

    public void SetData(TeacherData data, Action<TeacherCardCtrl> onClickCallback = null)
    {
        this.currentData = data;
        this._onClickCallback = onClickCallback;

        if (data == null)
        {
            if (lockImage != null) lockImage.SetActive(true);
            if (avatarImage != null) avatarImage.gameObject.SetActive(false);
            if (sexIconImage != null) sexIconImage.gameObject.SetActive(false);
            if (nameText != null) nameText.text = "";
            if (levelText != null) levelText.text = "";
            if (seniorityText != null) seniorityText.text = "";
            if (buffGoldText != null) buffGoldText.text = "";
            if (buffEducationText != null) buffEducationText.text = "";
            if (buffHappyText != null) buffHappyText.text = "";
            if (descGoldText != null) descGoldText.text = "";
            if (descEducationText != null) descEducationText.text = "";
            if (descHappyText != null) descHappyText.text = "";
            if (cardButton != null) cardButton.interactable = false;
            return;
        }

        if (lockImage != null) lockImage.SetActive(false);
        
        if (avatarImage != null)
        {
            avatarImage.gameObject.SetActive(true);
            avatarImage.sprite = data.avatar;
        }
        
        if (sexIconImage != null)
        {
            if (data.sexIcon != null)
            {
                sexIconImage.gameObject.SetActive(true);
                sexIconImage.sprite = data.sexIcon;
            }
            else
            {
                sexIconImage.gameObject.SetActive(false);
            }
        }

        if (cardButton != null) cardButton.interactable = true;
        if (nameText != null) nameText.text = data.teacherName;
        if (levelText != null) levelText.text = "Lv." + data.level.ToString();
        if (seniorityText != null) seniorityText.text = data.seniority.ToString() + " năm";
        
        if (buffGoldText != null) buffGoldText.text = "Vàng +" + data.influenceGold.ToString();
        if (buffEducationText != null) buffEducationText.text = "Học vấn +" + data.influenceEducation.ToString() + "%";
        if (buffHappyText != null) buffHappyText.text = "Hạnh phúc +" + data.influenceHappy.ToString() + "%";
        
        if (descGoldText != null) descGoldText.text = data.descGold;
        if (descEducationText != null) descEducationText.text = data.descEducation;
        if (descHappyText != null) descHappyText.text = data.descHappy;
    }

    private void OnClickCard()
    {
        if (_onClickCallback != null)
        {
            _onClickCallback.Invoke(this);
        }
    }
}
