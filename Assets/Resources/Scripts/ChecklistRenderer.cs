using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistRenderer : MonoBehaviour
{
    private VerticalLayoutGroup _verticalLayoutGroup;
    public int characterCount = 50;
    public int splitNameLimit = 20;
    public GameObject checkListParent;
    public GameObject checkPrefabAuto;
    public GameObject checkPrefabManual;
    public TMP_Text checksTMP;
    public GameObject checkInstaceManual;
    public GameObject checkInstaceAuto;
    void Start()
    {
        Checklist checklist = new Checklist();
        checklist.Checks.Add(new Check { Name = "Check 1", ExpectedValue = "Expected 1" });
        checklist.Checks.Add(new Check { Name = "Check 2", ExpectedValue = "Expected 2" });
        checklist.Checks.Add(new Check { Name = "Checkcheckcheckcheckcheck 3", ExpectedValue = "Expected 3 AAAAA aa :)" });
        var checkTexts = checklist.CheckTexts(characterCount, splitNameLimit);
        string allText = string.Join("\n", checkTexts);
        _verticalLayoutGroup = checkListParent.GetComponent<VerticalLayoutGroup>();
        
        foreach (var check in checklist.Checks)
        {
            checkInstaceManual = Instantiate(checkPrefabManual);
            checkInstaceManual.GetComponentInChildren<TMP_Text>().text = check.Text(characterCount, splitNameLimit);
            checkInstaceManual.transform.SetParent(checkListParent.transform, false);
            checkInstaceManual.GetComponentInChildren<TMP_Text>().rectTransform.offsetMin = new Vector2(-700, checkInstaceManual.GetComponentInChildren<TMP_Text>().rectTransform.offsetMin.y);
            checkInstaceManual.GetComponentInChildren<TMP_Text>().rectTransform.offsetMax = new Vector2(700, checkInstaceManual.GetComponentInChildren<TMP_Text>().rectTransform.offsetMax.y);
        }
    }

    void Update()
    {
        
    }
}
