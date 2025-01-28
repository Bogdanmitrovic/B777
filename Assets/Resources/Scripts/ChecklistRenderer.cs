#nullable enable
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistRenderer : MonoBehaviour
{
    public int characterCount = 50;
    public int splitNameLimit = 20;
    public GameObject checkPrefab;
    public GameObject topButtons;
    public GameObject bottomButtons;
    public GameObject checklistDone;
    public TextAsset jsonFile;
    
    private int _checklistIndex = 0;
    private List<Checklist> _normalChecklists;
    private Checklist? _currentChecklist;
    void Start()
    {
        // normal
        // itemovrd
        // chklovrd
        // chklreset
        Checklist checklist = new Checklist();
        checklist.Checks.Add(new Check ("Oxygen", "Tested 100%" ,  false));
        checklist.Checks.Add(new Check ("Flight instruments", "Heading ___, Altimeter ___" ,  false));
        checklist.Checks.Add(new Check ("Parking brake", "Set", true));
        checklist.Checks.Add(new Check ("Fuel Control Switches", "CUTOFF", true));
        // checkListParent.GetComponent<VerticalLayoutGroup>();
        _normalChecklists = new List<Checklist>
        {
            checklist
        };
        bottomButtons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(LoadNormalChecklist);
        bottomButtons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(OverrideCheck);
        bottomButtons.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OverrideChecklist);
        bottomButtons.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(ResetChecklist);
        // LoadNormalChecklist();
        // OverrideChecklist();
        // OnCheckSelect(0);
        // OverrideCheck();
        // ResetChecklist();
    }
    
    private void LoadNormalChecklist()
    {
        if(_checklistIndex is >= 11 or < 0)
        {
            _checklistIndex = 0;
        }
        LoadChecklist(_normalChecklists[_checklistIndex]);
        _checklistIndex++;
    }

    private void LoadChecklist(Checklist checklist)
    {
        _currentChecklist?.Unload();
        _currentChecklist = checklist;
        checklist.Load(checkPrefab, gameObject, characterCount, splitNameLimit);
        ChecklistNotDone();
    }

    public void OnCheckboxCheck(int index, bool value)
    {
        if (_currentChecklist?.IsDone() != true) return;
        ChecklistDone();
    }

    public void OnCheckSelect(int index)
    {
        _currentChecklist?.OnCheckSelect(index);
    }

    private void OverrideCheck()
    {
        _currentChecklist?.OverrideCheck();
        if (_currentChecklist?.IsDone() == true)
        {
            ChecklistDone();
        }
    }

    private void OverrideChecklist()
    {
        _currentChecklist?.OverrideChecklist();
        ChecklistDone();
    }

    private void ResetChecklist()
    {
        _currentChecklist?.Reset();
        ChecklistNotDone();
    }

    private void ChecklistDone()
    {
        checklistDone.SetActive(true);
        bottomButtons.transform.GetChild(1).gameObject.SetActive(false);
    }
    
    private void ChecklistNotDone()
    {
        checklistDone.SetActive(false);
        bottomButtons.transform.GetChild(1).gameObject.SetActive(true);
    }
    
    
     
}
