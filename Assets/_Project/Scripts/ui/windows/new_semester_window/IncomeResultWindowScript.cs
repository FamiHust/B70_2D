using UnityEngine;
using UnityEngine.UI;
using B70.Balance;

public class IncomeResultWindowScript : WindowScript
{
    public static IncomeResultWindowScript instance;

    [Header("UI References")]
    public Text TuitionIncomeText;
    public Text DiningIncomeText;
    public Text TotalGoldIncomeText;
    public Text TeacherRentCostText;
    public Button ConfirmButton;

    [Header("Animations")]
    public Animator anim;

    private SemesterBreakdown _bd;
    private int _semesterNumber;
    private float _happiness;
    private float _education;

    private void Awake()
    {
        instance = this;
        if (ConfirmButton != null)
        {
            ConfirmButton.onClick.AddListener(OnClickConfirm);
        }
    }

    public void Setup(SemesterBreakdown bd, int semesterNumber, float currentHappiness, float currentEducation)
    {
        _bd = bd;
        _semesterNumber = semesterNumber;
        _happiness = currentHappiness;
        _education = currentEducation;

        if (TuitionIncomeText != null)
            TuitionIncomeText.text = $"+{bd.semesterGoldIncome:F0}" + " vàng";

        if (DiningIncomeText != null)
            DiningIncomeText.text = $"+{bd.diningIncome:F0}" + " vàng";

        if (TeacherRentCostText != null)
        {
            if (bd.teacherRentCost > 0f)
                TeacherRentCostText.text = $"-{bd.teacherRentCost:F0}" + " vàng";
            else
                TeacherRentCostText.text = "+0 vàng";
        }

        float totalGold = bd.semesterGoldIncome + bd.diningIncome - bd.teacherRentCost;
        if (TotalGoldIncomeText != null)
        {
            if (totalGold >= 0f)
                TotalGoldIncomeText.text = $"+{totalGold:F0}" + " vàng";
            else
                TotalGoldIncomeText.text = $"-{Mathf.Abs(totalGold):F0}" + " vàng";
        }

        // ShowWindow();
    }

    public void OnClickConfirm()
    {
        // HideWindow();
        Close();
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowNewSemesterWindow(_bd, _semesterNumber, _happiness, _education);
        }
    }
}
