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
        checklist.Checks.Add(new Check { Name = "Oxygen", ExpectedValue = "Tested 100%" , isAutomatic = false});
        checklist.Checks.Add(new Check { Name = "Flight instruments", ExpectedValue = "Heading ___, Altimeter ___" , isAutomatic = false});
        checklist.Checks.Add(new Check { Name = "Parking brake", ExpectedValue = "Set", isAutomatic = true});
        checklist.Checks.Add(new Check { Name = "Fuel Control Switches", ExpectedValue = "CUTOFF", isAutomatic = true});
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
