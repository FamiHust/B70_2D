using UnityEngine;
using UnityEngine.UI;
using B70.Balance;

public class NewSemesterWindowScript : WindowScript
{
    public static NewSemesterWindowScript instance;

    [Header("UI References - Semester Info")]
    public Text SemesterTitleText;
    public Text SemesterNextText;
    public Text DropoutsText;
    public Text GraduatedText;

    [Header("Animations")]
    public Animator anim;

    private void Awake()
    {
        instance = this;
    }

    public void Setup(SemesterBreakdown bd, int semesterNumber, float currentHappiness, float currentEducation)
    {
        if (SemesterTitleText != null)
            SemesterTitleText.text = "Kết kỳ " + semesterNumber.ToString();

        if (SemesterNextText != null)
            SemesterNextText.text = (semesterNumber + 1).ToString();

        if (DropoutsText != null)
            DropoutsText.text = $"-{bd.dropouts:F0}";

        if (GraduatedText != null)
            GraduatedText.text = $"-{bd.graduated:F0}";

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

    private bool isClosing = false;

    public override void Close()
    {
        if (isClosing) return;
        isClosing = true;

        base.Close();
        
        if (UIManager.instance != null && UIManager.instance.masterTeacherCollection != null)
        {
            UIManager.instance.ShowCardSelectionWindow(UIManager.instance.masterTeacherCollection);
        }
    }
}
