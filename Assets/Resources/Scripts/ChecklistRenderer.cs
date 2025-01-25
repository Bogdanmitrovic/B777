using System.Collections.Generic;
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
    public GameObject checkPrefab;
    public TMP_Text checksTMP;
    private List<GameObject> _checkObjects;
    public GameObject checkInstaceAuto;
    void Start()
    {
        Checklist checklist = new Checklist();
        _checkObjects = new List<GameObject>();
        checklist.Checks.Add(new Check { Name = "Check 1", ExpectedValue = "Expected 1" });
        checklist.Checks.Add(new Check { Name = "Check 2", ExpectedValue = "Expected 2" });
        checklist.Checks.Add(new Check { Name = "Checkcheckcheckcheckcheck 3", ExpectedValue = "Expected 3 AAAAA aa :)" });
        var checkTexts = checklist.CheckTexts(characterCount, splitNameLimit);
        string allText = string.Join("\n", checkTexts);
        _verticalLayoutGroup = checkListParent.GetComponent<VerticalLayoutGroup>();

        for (var i = 0; i < checklist.Checks.Count; i++)
        {
            var check = checklist.Checks[i];
            var newObj = check.GetObj(checkPrefab, checkListParent, characterCount, splitNameLimit, i);
        }
    }

    public void OnCheckboxClick(int index, bool value)
    {
        Debug.Log("Kliknuo " + index + ": " + value);
    }

    void Update()
    {
        
    }
}
