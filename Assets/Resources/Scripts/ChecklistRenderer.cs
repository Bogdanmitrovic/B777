using TMPro;
using UnityEngine;

public class ChecklistRenderer : MonoBehaviour
{
    public int characterCount = 50;
    public int splitNameLimit = 20;
    public GameObject checkListParent;
    public GameObject checkPrefabAuto;
    public GameObject checkPrefabManual;
    public TMP_Text checksTMP;
    void Start()
    {
        Checklist checklist = new Checklist();
        checklist.Checks.Add(new Check { Name = "Check 1", ExpectedValue = "Expected 1" });
        checklist.Checks.Add(new Check { Name = "Check 2", ExpectedValue = "Expected 2" });
        checklist.Checks.Add(new Check { Name = "Checkcheckcheckcheckcheck 3", ExpectedValue = "Expected 3 AAAAA aa :)" });
        var checkTexts = checklist.CheckTexts(characterCount, splitNameLimit);
        string allText = string.Join("\n", checkTexts);
        checksTMP.text = allText;
    }

    void Update()
    {
        
    }
}
