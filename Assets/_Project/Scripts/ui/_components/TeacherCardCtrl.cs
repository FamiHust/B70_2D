using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class TeacherCardCtrl : MonoBehaviour
{
    [Header("UI References")]
    public GameObject lockImage;
    public Image avatarImage;
    public Image sexIconImage;
    public List<Text> nameText;
    public Text levelText;
    public Text statusText;
    public Text buffGoldText;
    public Text buffEducationText;
    public Text buffHappyText;
    public Text descGoldText;
    public Text descEducationText;
    public Text descHappyText;
    public Text priceText;
    public Button cardButton;

    [Header("Mode Toggle References")]
    public GameObject AvatarMode;
    public GameObject InfoMode;
    public Button InfoButton;

    public TeacherData currentData { get; private set; }

    private Action<TeacherCardCtrl> _onClickCallback;

    private void Awake()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnClickCard);
        }
        if (InfoButton != null)
        {
            InfoButton.onClick.AddListener(OnClickInfoButton);
        }
    }

    public void SetData(TeacherData data, Action<TeacherCardCtrl> onClickCallback = null)
    {
        this.currentData = data;
        this._onClickCallback = onClickCallback;

        if (data == null)
        {
            if (lockImage != null) lockImage.SetActive(true);
            if (AvatarMode != null) AvatarMode.SetActive(true);
            if (InfoMode != null) InfoMode.SetActive(false);
            if (avatarImage != null) avatarImage.gameObject.SetActive(false);
            if (sexIconImage != null) sexIconImage.gameObject.SetActive(false);
            if (nameText != null)
            {
                foreach (var text in nameText)
                {
                    if (text != null) text.text = "";
                }
            }
            if (levelText != null) levelText.text = "";
            if (statusText != null) statusText.text = "";
            if (buffGoldText != null) buffGoldText.text = "";
            if (buffEducationText != null) buffEducationText.text = "";
            if (buffHappyText != null) buffHappyText.text = "";
            if (descGoldText != null) descGoldText.text = "";
            if (descEducationText != null) descEducationText.text = "";
            if (descHappyText != null) descHappyText.text = "";
            if (priceText != null) priceText.text = "";
            if (cardButton != null) cardButton.interactable = false;
            return;
        }

        if (lockImage != null) lockImage.SetActive(false);
        if (AvatarMode != null) AvatarMode.SetActive(true);
        if (InfoMode != null) InfoMode.SetActive(false);
        
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
        if (nameText != null)
        {
            foreach (var text in nameText)
            {
                if (text != null) text.text = data.teacherName;
            }
        }
        if (levelText != null) levelText.text = "Lv." + data.level.ToString();
        if (statusText != null) statusText.text = "";
        
        if (buffGoldText != null)
        {
            string sign = data.influenceGold >= 0 ? "+ " : "- ";
            buffGoldText.text = "Vàng " + sign + Mathf.Abs(data.influenceGold).ToString() + "/h";
        }
        if (buffEducationText != null)
        {
            string sign = data.influenceEducation >= 0 ? "+ " : "- ";
            buffEducationText.text = "Học vấn " + sign + Mathf.Abs(data.influenceEducation).ToString() + "/h";
        }
        if (buffHappyText != null)
        {
            string sign = data.influenceHappy >= 0 ? "+ " : "- ";
            buffHappyText.text = "Hạnh phúc " + sign + Mathf.Abs(data.influenceHappy).ToString() + "/h";
        }
        
        if (descGoldText != null) descGoldText.text = data.descGold;
        if (descEducationText != null) descEducationText.text = data.descEducation;
        if (descHappyText != null) descHappyText.text = data.descHappy;
        if (priceText != null) priceText.text = data.hirePrice.ToString();
    }

    private void OnClickCard()
    {
        if (_onClickCallback != null)
        {
            _onClickCallback.Invoke(this);
        }
    }

    private void OnClickInfoButton()
    {
        if (AvatarMode != null && InfoMode != null)
        {
            bool isAvatarActive = AvatarMode.activeSelf;
            AvatarMode.SetActive(!isAvatarActive);
            InfoMode.SetActive(isAvatarActive);
        }
        else
        {
            if (AvatarMode != null) AvatarMode.SetActive(!AvatarMode.activeSelf);
            if (InfoMode != null) InfoMode.SetActive(!InfoMode.activeSelf);
        }
    }
}
