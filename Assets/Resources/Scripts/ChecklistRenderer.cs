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
    public GameObject buttonPrefab;
    public GameObject topButtons;
    public GameObject bottomButtons;
    public GameObject checklistDone;
    public GameObject title;
    public HorizontalLayoutGroup horizontalLayoutGroup;
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
        topButtons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ShowNormalMenu);
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

    public void ShowNormalMenu()
    {
        LoadChecklist(new Checklist());
        var verticalLayoutGroup1 = horizontalLayoutGroup.transform.GetChild(0);
        var verticalLayoutGroup2 = horizontalLayoutGroup.transform.GetChild(1);
        
        for (int i = verticalLayoutGroup1.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup1.GetChild(i).gameObject);
        }
        for (int i = verticalLayoutGroup2.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup2.GetChild(i).gameObject);
        }
        bottomButtons.SetActive(false);
        
        if (jsonFile != null)
        {
            Wrapper menus = JsonUtility.FromJson<Wrapper>(jsonFile.text);

            MenuItem menu = menus.Menus[0];
            title.SetActive(true);
            title.GetComponent<TMP_Text>().text = menu.MenuName.ToUpper();

            foreach (var list in menu.Lists)
            {
                GameObject button;
                if (verticalLayoutGroup1.childCount < 7)
                {
                    button = Instantiate(buttonPrefab, verticalLayoutGroup1.transform);
                }
                else
                {
                    button = Instantiate(buttonPrefab, verticalLayoutGroup2.transform);
                }
                button.transform.localScale = Vector3.one * 4;
                button.GetComponentInChildren<TMP_Text>().text = list.ListName;

                Checklist checklist = new Checklist();
                foreach (var item in list.List)
                {
                    checklist.Checks.Add(new Check(item.name, item.expectedValue, item.isAutomatic));
                }
                button.transform.GetComponent<Button>().onClick.AddListener(()=>
                {
                    ClearMenu();
                    LoadChecklist(checklist);
                    bottomButtons.SetActive(true);
                });
            }
        }
    }

    public void ClearMenu()
    {
        var verticalLayoutGroup1 = horizontalLayoutGroup.transform.GetChild(0);
        var verticalLayoutGroup2 = horizontalLayoutGroup.transform.GetChild(1);
        
        for (int i = verticalLayoutGroup1.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup1.GetChild(i).gameObject);
        }
        for (int i = verticalLayoutGroup2.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup2.GetChild(i).gameObject);
        }
    }

}
