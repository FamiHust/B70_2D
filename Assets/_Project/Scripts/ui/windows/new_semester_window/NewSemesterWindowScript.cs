using UnityEngine;
using UnityEngine.UI;
using B70.Balance;

public class NewSemesterWindowScript : WindowScript
{
    public static NewSemesterWindowScript instance;

    [Header("UI References - Semester Info")]
    public Text SemesterTitleText;
    public Text SemesterNextText;
    public Text FreshmenText;
    public Text DropoutsText;
    public Text GraduatedText;
    public Text DeltaStudentsText;
    public Text GoldIncomeText;
    public Text GraduationRateText;

    [Header("UI References - Stats")]
    public Text HappinessText;
    public Text EducationText;

    [Header("Animations")]
    public Animator anim;

    private void Awake()
    {
        instance = this;
    }

    public void Setup(SemesterBreakdown bd, int semesterNumber, float currentHappiness, float currentEducation)
    {
        if (SemesterTitleText != null)
            SemesterTitleText.text = semesterNumber.ToString();

        if (SemesterNextText != null)
            SemesterNextText.text = (semesterNumber + 1).ToString();

        if (FreshmenText != null)
            FreshmenText.text = $"+{bd.freshmen:F0}";

        if (DropoutsText != null)
            DropoutsText.text = $"-{bd.dropouts:F0}";

        if (GraduatedText != null)
            GraduatedText.text = $"-{bd.graduated:F0}";

        if (DeltaStudentsText != null)
        {
            string sign = bd.deltaStudents >= 0 ? "+" : "";
            DeltaStudentsText.text = $"{sign}{bd.deltaStudents:F0}";
        }

        if (GoldIncomeText != null)
            GoldIncomeText.text = $"+{bd.semesterGoldIncome:F0}";

        if (GraduationRateText != null)
            GraduationRateText.text = $"{bd.graduationRate:P1}";

        if (HappinessText != null)
            HappinessText.text = $"{currentHappiness:F0}/100";

        if (EducationText != null)
            EducationText.text = $"{currentEducation:F0}/100";

        // ShowWindow();
    }

    // public void OnClickContinue()
    // {
    //     HideWindow();
    //     Invoke("Close", 0.5f); // Wait for animation
    // }

    // public void HideWindow()
    // {
    //     if (anim != null) anim.Play("Hide");
    // }

    // public void ShowWindow()
    // {
    //     if (anim != null) anim.Play("Show");
    // }
}
